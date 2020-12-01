module Appstract.IntraPageAbstraction

open Appstract.DOM
open Appstract.Types
open System.Collections.Generic
open FSharp.Collections

type Tag = {
    Cluster: Cluster
    Depth: int
}

let computeTags (parentDict: Dictionary<Node, Node>) (leaves: Node list) (cluster: Cluster) : Map<Node, Map<Tag, int>> =
    let rec createTagsForBranch depth (map: Map<Node, Map<Tag, int>>) node =
        let parent = parentDict.[node]
        let tag = {Cluster = cluster; Depth = depth}
        let newTagCount = function
            | None -> Map([(tag, 1)])
            | Some tagCount -> tagCount |> Map.change tag (function None -> Some 1 | Some i -> Some (i+1))
        let newMap = map |> Map.change node (function tagCount -> Some (newTagCount tagCount))
        match (node, parent) with
        | (_, EmptyNode) -> newMap
        | _ -> createTagsForBranch (depth + 1) newMap node

    leaves |> List.fold (createTagsForBranch 0) Map.empty


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

    clusters |> List.fold createDictForOneCluster Map.empty
        

         
