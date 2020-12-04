module Appstract.IntraPageAbstraction

open Appstract.DOM
open Appstract.Types
open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices

type Tag = {
        Cluster: Cluster
        Depth: int
    } 

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
        

         
