module Appstract.IntraPageAbstraction

open System.Collections.Generic
open FSharp.Collections
open FSharpPlus

open Appstract.DOM
open Appstract.Types
open Appstract.Utils
open FSharpPlus

type Tag = { Cluster: Cluster; Depth: int }
type TagMap = Dictionary<Node, Dictionary<Tag, int>>
type MList<'T> = System.Collections.Generic.List<'T>

type Appstracter() =
    let mutable tagMap = TagMap()
    let computeTags (parentDict: Dictionary<Node, Node>) clusterMap leaves =
        let rec createTagsForBranch depth cluster node =
            let parent = parentDict.[node]
            let tag = { Cluster = cluster; Depth = depth }
            
            match tagMap.ContainsKey(node) with
            | true -> tagMap.[node] |> Dict.incrementAt tag
            | false -> tagMap.Add(node, Dictionary(dict [(tag, 1)]))
            
            match (node, parent) with
            | (_, EmptyNode) -> ()
            | _ -> createTagsForBranch (depth + 1) cluster parent

        leaves
        |> List.filter (fun leaf -> Map.containsKey leaf clusterMap)
        |> List.iter (fun leaf -> createTagsForBranch 0 clusterMap.[leaf] leaf)

    let computeClusters (parentDict: Dictionary<Node, Node>) (leaves: Node list): Map<Node, Cluster> =
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

    let abstractString (abstractValue: string) (concreteValue: string) =
        if abstractValue = concreteValue then concreteValue
        elif max abstractValue.Length concreteValue.Length > Settings.MAX_LCS then ""
        else String.LCS abstractValue concreteValue

    let abstractAttributes (attrs1: Attributes) (attrs2: Attributes) =
        let commonAttributes =
            Set.intersect (Map.keys attrs1 |> Set.ofSeq) (Map.keys attrs2 |> Set.ofSeq)

        commonAttributes
        |> Set.map (fun attrName -> (attrName, (attrs1.[attrName], attrs2.[attrName])))
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
    let groupByTagAssociatively nodes =
        let filteredTagMap =
            tagMap
            |> Dict.toList
            |> List.map (fun (node, tags) -> (node, tags |> Dict.keys |> Set.ofSeq))
            |> List.filter (fun (n, _) -> nodes |> List.contains n)

        let mutable tagGroups = Dictionary<Tag, Set<Tag>>()
        let createGroups (tags: Set<Tag>) =
            let addTagsToTagsGroup tags =
                tags |> Set.iter (fun tag -> tagGroups.[tag] <- tags)

            let findFirstTagInTagGroup tags =
                tags |> Set.toList |> List.tryFind (fun tag -> tagGroups.ContainsKey(tag))

            match tags |> findFirstTagInTagGroup with
            | None -> addTagsToTagsGroup tags
            | Some key ->
                tagGroups.[key]
                |> Set.union tags
                |> addTagsToTagsGroup

        filteredTagMap
        |> List.map snd
        |> List.iter createGroups 

        let mutable tagsToNodes = Dictionary<Set<Tag>, MList<Node>>()
        filteredTagMap
        |> List.map (fun (node, group) -> (node, group |> Set.toList |> List.head))
        |> List.iter (fun (node, tag) ->
            let key = tagGroups.[tag] 
            match tagsToNodes |> Dict.tryFind key with
            | None -> tagsToNodes.Add(key, MList<Node>(Seq.singleton node))
            | Some nodeList -> nodeList.Add(node)
            )
        
        let res =
            tagsToNodes
            |> Dict.toList
            |> List.map (fun (tags, nodes) -> (tags, nodes |> Set.ofSeq))
        
        res

    let groupPairsOfChildrenWithinTheSameCluster (nodes1: Node List) (nodes2: List<Node>) =
        let getTags node =
            tagMap
            |> Dict.tryFind node
            |> Option.map (Dict.keys >> Set.ofSeq)
            |> Option.defaultValue Set.empty

        let concatTags = (Seq.collect getTags) >> Set.ofSeq

        let commonTags =
            Set.intersect (concatTags nodes1) (concatTags nodes2)

        let isOrphan =
            getTags >> Set.intersect commonTags >> Set.isEmpty

        let orphans =
            nodes1
            |> List.append nodes2
            |> List.filter isOrphan
            |> Set.ofList

        let getCommonTags node1 node2 =
            Set.intersect (getTags node1) (getTags node2)

        let pairs =
            List.allPairs (nodes1 |> List.filter (isOrphan >> not)) (nodes2 |> List.filter (isOrphan >> not))
            |> List.fold (fun map (n1, n2) -> map |> Map.add (n1, n2) (getCommonTags n1 n2)) Map.empty
            |> Map.filter (fun nodesPair commonTags -> not commonTags.IsEmpty)
            |> Map.toList
            |> List.map (fun ((n1, n2), tags) -> (n1, n2, tags))

        (pairs, orphans)

    let copyTags source destination =
        if tagMap |> Dict.containsKey source
        then tagMap.[destination] <- tagMap.[source]

    let updateOrphanNode node =
        let newNode = updateType Optional node
        copyTags node newNode
        newNode

    let rec mergeAbstractTrees (node1: Node, node2: Node, commonTags: Set<Tag>): Node =
        let (pairs, orphans) =
            groupPairsOfChildrenWithinTheSameCluster (node1.Children()) (node2.Children())

        let mergedChildren = pairs |> List.map mergeAbstractTrees 

        let orphans = orphans |> Seq.map updateOrphanNode |> Seq.toList

        let children = orphans |> Seq.append mergedChildren |> Seq.toList

        let newNode =
            match (node1, node2) with
            | (Text (t1, data1), Text (t2, data2)) -> Text(abstractString t1 t2, mergeAbstractionData data1 data2)

            | (Element (tag1, attrs1, data1, _), Element (tag2, attrs2, data2, _)) ->
                Element(tag1, abstractAttributes attrs1 attrs2, mergeAbstractionData data1 data2, children)

            | _ -> EmptyNode

        let commonTagsWithOccurenceCount =
            commonTags
            |> Set.toList
            |> List.map (fun tag -> (tag, 1))
            |> Dict.ofSeq

        tagMap.[newNode] <- commonTagsWithOccurenceCount

        newNode

    let isAbstract node =
        match tagMap |> Dict.tryFind node with
        | None -> true
        | Some tags ->
            tags
            |> Dict.exists (fun _ nbOccurenceOfTag -> nbOccurenceOfTag >= Settings.MIN_SIZE_CLUSTER)
            |> not

    let mergeGroup (commonTags, nodes) =
        let rec reduce reducedNode =
            function
            | first :: rest ->
                let reducedNode = mergeAbstractTrees (reducedNode, first, commonTags)
                reduce reducedNode rest
            | [] -> reducedNode

        let nodes = nodes |> Set.toList
        match nodes with
        | first::[] -> reduce first []
        | first::rest ->
            let node = reduce first rest
            let newNode = node.UpdateAbstractionType (mergeAbstractionType OneToMany)
            copyTags node newNode 
            newNode
        | [] -> failwith "Cannot merge empty groups"

    let rec abstractTree node: Node =
        match node with
        | EmptyNode
        | Text (_)
        | Element (_) when node |> isAbstract -> node

        | Element (name, attrs, data, children) ->
            let children = children |> List.map abstractTree 

            let children =
                groupByTagAssociatively children
                |> List.map mergeGroup
                
            let newElement = Element(name, attrs, data, children)
            copyTags node newElement
            newElement

    member this.ComputeClusters = computeClusters
    member this.ComputeTags = computeTags
    member this.TagMap = tagMap
    member this.AbstractTree = abstractTree
    
let appstract (tree: Node): Node =
    let parentDict = tree.ParentDict()
    let leaves = tree.Nodes() |> List.filter isLeaf

    let appstracter = Appstracter()
    let clusterMap = appstracter.ComputeClusters parentDict leaves
    appstracter.ComputeTags parentDict clusterMap leaves

    let result = appstracter.AbstractTree tree
    result
