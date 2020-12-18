module Appstract.IntraPageAbstraction

open System.Threading.Tasks
open Appstract.DOM
open Appstract.Types
open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices
open Appstract.Utils
open FSharpPlus.Operators

type Tag = { Cluster: Cluster; Depth: int }
type TagMap = Map<Node, Map<Tag, int>>

let computeTags (parentDict: IDictionary<Node, Node>) clusterMap leaves =
    let rec createTagsForBranch depth cluster (map: TagMap) node =
        let parent = parentDict.[node]
        let tag = { Cluster = cluster; Depth = depth }

        let newTagCount =
            function
            | None -> Map([ (tag, 1) ])
            | Some tagCount ->
                tagCount
                |> Map.change tag (function
                       | None -> Some 1
                       | Some i -> Some(i + 1))

        let newMap =
            map
            |> Map.change node (function
                   | tagCount -> Some(newTagCount tagCount))

        match (node, parent) with
        | (_, EmptyNode) -> newMap
        | _ -> createTagsForBranch (depth + 1) cluster newMap parent

    leaves
    |> Seq.filter (fun leaf -> Map.containsKey leaf clusterMap)
    |> Seq.fold (fun m leaf -> createTagsForBranch 0 clusterMap.[leaf] m leaf) Map.empty

let computeClusters parentDict (leaves: Node list): Map<Node, Cluster> =
    let getNodePath = computeRootFromLeafPath parentDict

    let toCluster nodes = Cluster(Set(nodes))

    let clusters =
        leaves
        |> List.map (fun n -> (n, getNodePath n))
        |> List.groupBy snd
        |> List.map (snd >> List.map fst >> toCluster)

    let createDictForOneCluster dict cluster =
        let createDictForOneNode d node = d |> Map.add node cluster
        let (Cluster (nodes)) = cluster
        nodes |> Set.fold createDictForOneNode dict

    clusters
    |> List.fold createDictForOneCluster Map.empty
    |> Map.filter (fun _ (Cluster (nodes)) -> not nodes.IsEmpty)

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

let groupByTagAssociatively (tagMap: TagMap) nodes =
    let filteredTagMap =
        tagMap
        |> Map.toSeq
        |> Seq.map (fun (node, tags) -> (node, tags |> Map.keys))
        |> Seq.filter (fun (n, _) -> nodes |> Seq.contains n)

    let createGroups tagGroups tags =
        let addTagsToTagsGroup =
            Seq.fold (fun m tag -> m |> Map.add tag tags) tagGroups

        let findFirstTagInTagGroup =
            Seq.tryFind (fun tag -> Map.containsKey tag tagGroups)

        match tags |> findFirstTagInTagGroup with
        | None -> addTagsToTagsGroup tags
        | Some key ->
            tagGroups.[key]
            |> Set.union tags
            |> addTagsToTagsGroup

    let groups =
        filteredTagMap
        |> Seq.map snd
        |> Seq.fold createGroups Map.empty

    filteredTagMap
    |> Seq.map (fun (node, group) -> (node, Seq.head group))
    |> Seq.fold (fun map (node, tag) -> map |> Map.push groups.[tag] (Set.singleton node)) Map.empty
    |> Map.toSeq

let groupPairsOfChildrenWithinTheSameCluster (tagMap: TagMap) (nodes1: Node seq) (nodes2: seq<Node>) =
    let getTags node =
        tagMap
        |> Map.map (fun _ tags -> tags |> Map.keys)
        |> Map.tryFind node
        |> Option.defaultValue Set.empty

    let concatTags = (Seq.collect getTags) >> Set.ofSeq

    let commonTags =
        Set.intersect (concatTags nodes1) (concatTags nodes2)

    let isOrphan =
        getTags >> Set.intersect commonTags >> Set.isEmpty

    let orphans =
        nodes1
        |> Seq.append nodes2
        |> Seq.filter isOrphan
        |> Set.ofSeq

    let getCommonTags node1 node2 =
        Set.intersect (getTags node1) (getTags node2)

    let pairs =
        Seq.allPairs (nodes1 |> Seq.filter (isOrphan >> not)) (nodes2 |> Seq.filter (isOrphan >> not))
        |> Seq.fold (fun map (n1, n2) -> map |> Map.add (n1, n2) (getCommonTags n1 n2)) Map.empty
        |> Map.filter (fun nodesPair commonTags -> not commonTags.IsEmpty)
        |> Map.toSeq
        |> Seq.map (fun ((n1, n2), tags) -> (n1, n2, tags))

    (pairs, orphans)

let abstractString (abstractValue: string) (concreteValue: string) =
    if abstractValue = concreteValue then concreteValue
    elif max abstractValue.Length concreteValue.Length > Settings.MAX_LCS then ""
    else String.LCS abstractValue concreteValue

let abstractAttributes (attrs1: Attributes) (attrs2: Attributes) =
    let commonAttributes =
        Set.intersect (Map.keys attrs1) (Map.keys attrs2)

    commonAttributes
    |> Seq.map (fun attrName -> (attrName, (attrs1.[attrName], attrs1.[attrName])))
    |> Map.ofSeq
    |> Map.map (fun attrName (m1Value, m2Value) -> abstractString m1Value m2Value)


let mergeAbstractionType type1 type2 =
    let rec computeType typePair =
        match typePair with
        | (t1, t2) when t1 = t2 -> t1
        | (t1, Normal) -> t1
        | (Optional, ZeroToMany) -> ZeroToMany
        | (Optional, OneToMany) -> ZeroToMany
        | (OneToMany, ZeroToMany) -> ZeroToMany
        | (t1, t2) -> computeType (t2, t1)

    computeType (type1, type2)

let mergeAbstractionData data1 data2 =
    { AbstractionType = mergeAbstractionType data1.AbstractionType data2.AbstractionType
      Source = Set.union data1.Source data2.Source }

let updateType abstractionType (node: Node) =
    node.UpdateAbstractionType(mergeAbstractionType abstractionType)

let copyTags source destination tagMap =
    if tagMap |> Map.containsKey source
    then tagMap |> Map.add destination tagMap.[source]
    else tagMap

let updateOrphanNode map node =
    let newNode = updateType Optional node
    let newTagMap = map |> copyTags node newNode
    (newNode, newTagMap)

let rec mergeAbstractTrees (tagMap: TagMap) (node1: Node, node2: Node, commonTags: Set<Tag>): Node * TagMap =
    let (pairs, orphans) =
        groupPairsOfChildrenWithinTheSameCluster tagMap (node1.Children()) (node2.Children())

    let mergedChildren, tagMap =
        pairs |> Seq.mapFold mergeAbstractTrees tagMap

    let orphans, tagMap =
        orphans |> Seq.mapFold updateOrphanNode tagMap

    let children =
        orphans |> Seq.append mergedChildren |> List.ofSeq

    let newNode =
        match (node1, node2) with
        | (Text (t1, data1), Text (t2, data2)) -> Text(abstractString t1 t2, mergeAbstractionData data1 data2)

        | (Element (tag1, attrs1, data1, _), Element (tag2, attrs2, data2, _)) when tag1 = tag2 ->
            Element(tag1, abstractAttributes attrs1 attrs2, mergeAbstractionData data1 data2, children)

        | _ -> EmptyNode

    let commonTagsWithOccurenceCount =
        commonTags
        |> Set.toSeq
        |> Seq.map (fun tag -> (tag, 1))
        |> Map.ofSeq

    let tagMap =
        tagMap
        |> Map.add newNode commonTagsWithOccurenceCount

    newNode, tagMap

let MIN_SIZE_ABSTRACT = 2

let isAbstract (tagMap: TagMap) node =
    match tagMap |> Map.tryFind node with
    | None -> true
    | Some tags ->
        tags
        |> Map.exists (fun _ nbOccurenceOfTag -> nbOccurenceOfTag >= MIN_SIZE_ABSTRACT)
        |> not

let mergeGroup tagMap (commonTags, nodes) =
    let rec reduce tagMap reducedNode =
        function
        | first :: rest ->
            let reducedNode, tagMap =
                mergeAbstractTrees tagMap (reducedNode, first, commonTags)

            reduce tagMap reducedNode rest
        | [] -> reducedNode, tagMap

    let nodes = nodes |> Set.toList
    match nodes with
    | first::[] -> reduce tagMap first []
    | first::rest ->
        let node, tagMap = reduce tagMap first rest
        let node = node.UpdateAbstractionType (mergeAbstractionType OneToMany)
        node, tagMap
    | [] -> failwith "Cannot merge empty groups"

let rec abstractTree tagMap node: Node * TagMap =
    match node with
    | EmptyNode
    | Text (_)
    | Element (_) when node |> (isAbstract tagMap) -> node, tagMap

    | Element (name, attrs, data, children) ->
        let children, tagMap =
            children |> Seq.mapFold abstractTree tagMap

        let children, tagMap =
            groupByTagAssociatively tagMap children
            |> Seq.mapFold mergeGroup tagMap
        
        let newElement = Element(name, attrs, data, children |> Seq.toList)
        let tagMap = tagMap |> copyTags node newElement
        newElement, tagMap

let intraPageAbstraction (tree: Node): Node =
    let parentDict = tree.ParentDict()
    let leaves = tree.Nodes() |> List.filter isLeaf

    let clusterMap = computeClusters parentDict leaves
    let tagMap = computeTags parentDict clusterMap leaves

    let result, _ = abstractTree tagMap tree
    result
