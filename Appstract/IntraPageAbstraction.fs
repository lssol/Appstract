module Appstract.IntraPageAbstraction

open System.Collections.Generic
open System.Linq
open FSharp.Collections
open FSharpPlus

open Appstract.DOM
open Appstract.Types
open Appstract.Utils
open FSharpPlus
open Nest

type Tag = { Cluster: Cluster; Depth: int }
type TagMap = Dictionary<Node, Dictionary<Tag, int>>
type MList<'T> = System.Collections.Generic.List<'T>
type NSet = HashSet<Node>

type Appstracter() =
    let mutable tagMap = TagMap()

    let computeTags (parentDict: ParentDict) clusterMap leaves =
        let rec createTagsForBranch depth cluster node =
            let parent = parentDict.[node]
            let tag = { Cluster = cluster; Depth = depth }

            match tagMap.ContainsKey(node) with
            | true -> tagMap.[node] |> Dict.incrementAt tag
            | false -> tagMap.Add(node, Dictionary(dict [ (tag, 1) ]))

            match parent with
            | Some p -> createTagsForBranch (depth + 1) cluster p
            | None -> ()

        leaves
        |> Array.filter (fun leaf -> Map.containsKey leaf clusterMap)
        |> Array.iter (fun leaf -> createTagsForBranch 0 clusterMap.[leaf] leaf)

    let computeClusters (parentDict: ParentDict) (leaves: Node array): Map<Node, Cluster> =
        let getNodePath = computeRootFromLeafPath parentDict

        let toCluster (nodes: Node array) = Cluster(NSet(nodes))

        let clusters =
            leaves
            |> Array.map (fun n -> (n, getNodePath n))
            |> Array.groupBy snd
            |> Array.map (snd >> Array.map fst >> toCluster)

        let createDictForOneCluster dict cluster =
            let createDictForOneNode d node = d |> Map.add node cluster
            let (Cluster (nodes)) = cluster
            nodes.ToArray() |> Array.fold createDictForOneNode dict

        clusters
        |> Array.fold createDictForOneCluster Map.empty
        |> Map.filter (fun _ (Cluster (nodes)) -> not (nodes.Count = 0))

    let extractBoxes (parentDict: ParentDict) (cluster: Cluster) =
        let extractBoxesFromLeaf (leaf: Node) =
            let boxes = Dictionary<Node, int>()
            let mutable currentCount = 1

            let addIfBox n prev depth =
                match prev with
                | None -> ()
                | Some prev ->
                    let tag = { Cluster = cluster; Depth = depth }
                    tagMap
                    |> Dict.tryFind n
                    |> Option.iter (fun tags ->
                        tags
                        |> Dict.tryFind tag
                        |> Option.defaultValue 0
                        |> (fun i ->
                            let diff = i - currentCount
                            if diff > 0 then boxes.Add(prev, diff)))

            leaf.ActOnBranch parentDict addIfBox
            boxes

        let (Cluster nodesInCluster) = cluster
        nodesInCluster.ToArray()
        |> Array.map (fun node -> (node, extractBoxesFromLeaf node))
        |> Dict.ofSeq

    let abstractString (abstractValue: string) (concreteValue: string) =
        if abstractValue = concreteValue then concreteValue
        elif max abstractValue.Length concreteValue.Length > Settings.MAX_LCS then ""
        else String.LCS abstractValue concreteValue

    let mergeAttributes (attrs1: Attributes) (attrs2: Attributes) =
        let commonAttributes =
            Set.intersect (Map.keys attrs1 |> Set.ofSeq) (Map.keys attrs2 |> Set.ofSeq)

        commonAttributes
        |> Seq.map (fun attrName -> (attrName, (attrs1.[attrName], attrs2.[attrName])))
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
        { abstractionType = mergeAbstractionType data1.abstractionType data2.abstractionType
          source = Set.union data1.source data2.source }

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

    let groupByTagAssociatively (nodes: NSet) =
        let filteredTagMap =
            tagMap
            |> Dict.toSeq
            |> Seq.filter (fun (n, _) -> nodes.Contains n)
            |> Seq.map (fun (node, tags) -> (node, tags.Keys.ToHashSet()))
            |> Seq.toArray

        let mutable tagGroups = Dictionary<Tag, HashSet<Tag>>()

        let createGroups (tags: HashSet<Tag>) =
            let addTagsToTagsGroup (tags: HashSet<Tag>) =
                tags.ToArray()
                |> Array.iter (fun tag -> tagGroups.[tag] <- tags)

            let findFirstTagInTagGroup (tags: HashSet<Tag>) =
                tags.ToArray()
                |> Array.tryFind (fun tag -> tagGroups.ContainsKey(tag))

            match tags |> findFirstTagInTagGroup with
            | None -> addTagsToTagsGroup tags
            | Some key ->
                tagGroups.[key].Union(tags).ToHashSet()
                |> addTagsToTagsGroup

        filteredTagMap
        |> Array.iter (snd >> createGroups)

        let mutable tagsToNodes = Dictionary<HashSet<Tag>, MList<Node>>()
        filteredTagMap
        |> Array.map (fun (node, group) -> (node, group |> Seq.head))
        |> Array.iter (fun (node, tag) ->
            let key = tagGroups.[tag]
            match tagsToNodes |> Dict.tryFind key with
            | None -> tagsToNodes.Add(key, MList<Node>(Seq.singleton node))
            | Some nodeList -> nodeList.Add(node))

        let res =
            tagsToNodes
            |> Dict.toArray
            |> Array.map (fun (tags, nodes) -> (tags, nodes |> Set.ofSeq))

        res

    let groupPairsOfChildrenWithinTheSameCluster (nodes1: Node array) (nodes2: Node array) =
        let getTags node =
            tagMap
            |> Dict.tryFind node
            |> Option.map (fun dict -> dict.Keys.ToHashSet())
            |> Option.defaultValue (HashSet<Tag>())

        let concatTags (nodes: Node array) = (Seq.collect getTags nodes).ToHashSet()

        let commonTags = HashSet.intersect (concatTags nodes1) (concatTags nodes2)
        let isOrphan = getTags >> HashSet.intersect commonTags >> Seq.isEmpty

        let orphans =
            nodes1
            |> Array.append nodes2
            |> Array.filter isOrphan
            |> Set.ofArray

        let getCommonTags node1 node2 = HashSet.intersect (getTags node1) (getTags node2)
        
        let pairs =
            Array.allPairs (nodes1 |> Array.filter (isOrphan >> not)) (nodes2 |> Array.filter (isOrphan >> not))
            |> Array.fold (fun map (n1, n2) -> map |> Map.add (n1, n2) (getCommonTags n1 n2)) Map.empty
            |> Map.filter (fun nodesPair commonTags -> commonTags.Any())
            |> Map.toArray
            |> Array.map (fun ((n1, n2), tags) -> (n1, n2, tags))

        (pairs, orphans)

    let copyTags source destination =
        if tagMap |> Dict.containsKey source then tagMap.[destination] <- tagMap.[source]

    let rec mergeAbstractTrees (node1: Node, node2: Node, commonTags: HashSet<Tag>): Node =
        let (pairs, orphans) =
            groupPairsOfChildrenWithinTheSameCluster (node1.children) (node2.children)

        let mergedChildren = pairs |> Array.map mergeAbstractTrees

        orphans
        |> Set.iter (fun n -> n.abstractionData.abstractionType <- AbstractionType.Optional)

        let children =
            mergedChildren
            |> Array.append (orphans |> Seq.toArray)

        let newNode =
            { signature = NodeId.Gen().ToString()
              name = node1.name
              attributes = mergeAttributes node1.attributes node2.attributes
              abstractionData = mergeAbstractionData node1.abstractionData node2.abstractionData
              children = children }

        let commonTagsWithOccurenceCount =
            commonTags
            |> Seq.map (fun tag -> (tag, 1))
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
                let reducedNode =
                    mergeAbstractTrees (reducedNode, first, commonTags)

                reduce reducedNode rest
            | [] -> reducedNode

        let nodes = nodes |> Set.toList
        match nodes with
        | first :: [] -> reduce first []
        | first :: rest ->
            let node = reduce first rest
            node.abstractionData.abstractionType <-
                mergeAbstractionType node.abstractionData.abstractionType OneToMany

            node
        | [] -> failwith "Cannot merge empty groups"

    let rec abstractTree node: Node =
        if node |> isAbstract then
            node
        else
            let children = node.children |> Array.map abstractTree
            let children =
                groupByTagAssociatively (children.ToHashSet())
                |> Array.map mergeGroup

            let newElement = { node with children = children }
            copyTags node newElement
            newElement

    member this.ComputeClusters = computeClusters
    member this.ComputeTags = computeTags
    member this.TagMap = tagMap
    member this.AbstractTree = abstractTree
    member this.ExtractBoxes = extractBoxes

let appstract (tree: Node): Node =
    let parentDict = tree.ParentDict()
    let nodes = tree.Nodes()
    let leaves = nodes |> Array.filter isLeaf

    let appstracter = Appstracter()

    let clusterMap =
        appstracter.ComputeClusters parentDict leaves

    appstracter.ComputeTags parentDict clusterMap leaves

    let result = appstracter.AbstractTree tree
    result
