module Appstract.IntraPageAbstraction

open System.Threading.Tasks
open Appstract.DOM
open Appstract.Types
open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices
open Appstract.Utils

type Tag = { Cluster: Cluster; Depth: int } 

let computeTags (parentDict: Dictionary<Node, Node>) (clusterMap: Map<Node, Cluster>) (leaves: Node seq) : Map<Node, Map<Tag, int>> =
    let rec createTagsForBranch depth cluster (map: Map<Node, Map<Tag, int>>) node =
        let parent = parentDict.[node]
        let tag = {Cluster = cluster; Depth = depth;}
        let newTagCount = function
            | None -> Map([(tag, 1)])
            | Some tagCount -> tagCount |> Map.change tag (function None -> Some 1 | Some i -> Some (i+1))
        let newMap = map |> Map.change node (function tagCount -> Some (newTagCount tagCount))
        match (node, parent) with
        | (_, EmptyNode) -> newMap
        | _ -> createTagsForBranch (depth + 1) cluster newMap parent

    leaves 
    |> Seq.filter (fun leaf -> Map.containsKey leaf clusterMap)
    |> Seq.fold (fun m leaf -> createTagsForBranch 0 clusterMap.[leaf] m leaf) Map.empty

let computeClusters (root: Node): Map<Node, Cluster> =
    let nodes = root.Nodes()
    let getNodePath = computeRootFromLeafPath (root.ParentDict())
    let toCluster nodes = Cluster(Set(nodes))
    let clusters = 
        nodes 
        |> List.filter isLeaf
        |> List.map (fun n -> (n, getNodePath n))
        |> List.groupBy snd
        |> List.map (snd >> List.map fst >> toCluster)

    let createDictForOneCluster dict cluster =
        let createDictForOneNode d node = d |> Map.add node cluster
        let (Cluster(nodes)) = cluster
        nodes |> Set.fold createDictForOneNode dict

    clusters 
    |> List.fold createDictForOneCluster Map.empty
    |> Map.filter (fun _ (Cluster(nodes)) -> not nodes.IsEmpty)
        
(*
       Allows to put all children of anode within DISJOINT tag groups.
       Without doing it, the problem is that we can have a node belonging to several tag groups (a container of multiple clusters)
       This node would then be abstracted more than once
       
       This method associatively groups the nodes within disjoint tag groups
       Ex: 
       Input:
       A belongs to t1, t2 
       B belongs to t2, t3 
       C belongs to t3 
       D belongs to t4 
       E belongs to t4
       
       Output:
       [A, B, C] -> [t1, t2, t3]
       [D, E] -> [t4]
*)
let groupByTagAssociatively (tagsPerNode: (Node * Set<Tag>) seq) =
    let createGroups tagGroups tags = 
        let addTagsToTagsGroup     = Seq.fold (fun m tag -> m |> Map.add tag tags) tagGroups
        let findFirstTagInTagGroup = Seq.tryFind (fun tag -> Map.containsKey tag tagGroups)

        match tags |> findFirstTagInTagGroup with
        | None -> addTagsToTagsGroup tags
        | Some key -> 
            tagGroups.[key] 
            |> Set.union tags
            |> addTagsToTagsGroup
        
    let groups = tagsPerNode |> Seq.map snd |> Seq.fold createGroups Map.empty
    tagsPerNode 
    |> Seq.map (fun (node, group) -> (node, Seq.head group))
    |> Seq.fold (fun map (node, tag) -> map |> Map.push groups.[tag] node) Map.empty
    |> Map.toSeq

let groupPairsOfChildrenWithinTheSameCluster (tagsPerNode: Map<Node, Set<Tag>>) (nodes1:Node seq) nodes2 =
    let getTags node = tagsPerNode |> Map.tryFind node |> Option.defaultValue Set.empty
    let concatTags = (Seq.collect getTags) >> Set.ofSeq
    let commonTags = Set.intersect (concatTags nodes1) (concatTags nodes2)
    let isOrphan = getTags >> Set.intersect commonTags >> Set.isEmpty
        
    let orphans = nodes1 |> Seq.append nodes2 |> Seq.filter isOrphan |> Set.ofSeq
    let getCommonTags node1 node2 = Set.intersect (getTags node1) (getTags node2)
    let pairs =
        Seq.allPairs
            (nodes1 |> Seq.filter (isOrphan >> not))
            (nodes2 |> Seq.filter (isOrphan >> not))
        |> Seq.fold (fun map (n1, n2) -> map |> Map.add (n1, n2) (getCommonTags n1 n2)) Map.empty
        |> Map.filter (fun nodesPair commonTags -> not commonTags.IsEmpty)
    (pairs, orphans)
    
let abstractString (abstractValue: string) (concreteValue: string) =
    if abstractValue = concreteValue then concreteValue
    elif max abstractValue.Length concreteValue.Length > Settings.MAX_LCS then ""
    else String.LCS abstractValue concreteValue
    
let abstractAttributes (attrs1: Attribute seq) (attrs2: Attribute seq) =
    let m1 = Attribute.toMap attrs1
    let m2 = Attribute.toMap attrs2
    let commonAttributes = Set.intersect (Map.keys m1) (Map.keys m2)
    commonAttributes
    |> Set.toSeq
    |> Seq.map (fun attrName -> (attrName, (m1.[attrName], m2.[attrName])))
    |> Map.ofSeq
    |> Map.map (fun attrName (m1Value, m2Value) -> abstractString m1Value m2Value)
    |> Attribute.ofMap

type AbstractionType = Normal | Optional | ZeroToMany | OneToMany
type AbstractionData = {
    AbstractionType: AbstractionType
    Source: Node seq
} with static member Default = {AbstractionType = Normal; Source = Seq.empty}

let computeAbstractionType type1 type2 =
    let rec computeType typePair =
        match typePair with
        | (t1, t2) when t1 = t2 -> t1
        | (t1, Normal) -> t1
        | (Optional, ZeroToMany) -> ZeroToMany
        | (Optional, OneToMany) -> ZeroToMany
        | (OneToMany, ZeroToMany) -> ZeroToMany
        | (t1, t2) -> computeType (t2, t1)
    computeType (type1.AbstractionType, type2.AbstractionType)
    
let rec mergeAbstractTrees (data: Map<Node, AbstractionData>) node1 node2 =
    let getData node = data |> Map.findOrDefault AbstractionData.Default node
    let newNode =
        match (node1, node2) with
        | (Text t1, Text t2) -> Text(abstractString t1 t2)
        | (Element(tag1, attrs1, _), Element(tag2, attrs2, _)) when tag1 = tag2 ->
            Element(tag1, abstractAttributes attrs1 attrs2, Seq.empty)
        | _ -> EmptyNode
    let newData =
        data
        |> Map.change
    
    let (pairs, orphans) = groupPairsOfChildrenWithinTheSameCluster tree1 tree2
    
    let (newChildrenFromPairs, abstractionData) =
        let folder (children, data) (node1, node2) =
            let newType = computeAbstractionType node1 node2
            let data
            let (newData, newNode) = mergeAbstractTrees abstractionData node1 node2
        Seq.fold
    
let toTagsPerNode tagMap: (Node * Set<Tag>) seq =
    let getKeys = Map.toSeq >> Seq.map fst >> Set.ofSeq
    tagMap 
    |> Map.toSeq 
    |> Seq.map (fun (node, tags) -> (node, tags |> getKeys))






