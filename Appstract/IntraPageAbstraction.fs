module Appstract.IntraPageAbstraction

open Appstract.DOM
open Appstract.Types
open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices
open Appstract.Utils

type Tag = { Cluster: Cluster; Depth: int } 

let computeTags (parentDict: Dictionary<Node, Node>) (clusterMap: Map<Node, Cluster>) (leaves: Node seq)  : Map<Node, Map<Tag, int>> =
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
type GroupState = {
    TagToGroup: Map<Tag, Set<Tag>>
    GroupToNode: Map<Set<Tag>, Tag>
}
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

// To use for later before calling the groupByTagsAssociatively
let toTagsPerNode tagMap =
    let getKeys = Map.toSeq >> Seq.map fst >> Set.ofSeq
    tagMap 
    |> Map.toSeq 
    |> Seq.map (fun (node, tags) -> (node, tags |> getKeys))






