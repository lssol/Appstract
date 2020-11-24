module Appstract.Tests.IntraPageAbstraction

open Appstract.DOM
open Appstract.Types
open System.Collections.Generic

let computeClusters (root: Node): ClusterData =
    let nodes = root.Nodes()
    let getNodePath = computeRootFromLeafPath (root.ParentDict())
    let nodeToPathTuple n = (n, getNodePath n)
    let toCluster (nodes: Node list) = Cluster(HashSet<Node>(nodes))
    let clusters = 
        nodes 
        |> List.map nodeToPathTuple
        |> List.groupBy snd
        |> List.map (fun (key, tupleList) -> tupleList)
        |> List.map (List.map (fun (node, path) -> node))
        |> List.map toCluster

    let clusterDict = Dictionary<Node, Cluster>()
    let fillDict cluster = 
        let (Cluster(nodes)) = cluster
        nodes |> Seq.iter (fun n -> clusterDict.Add(n, cluster))
    clusters |> List.iter fillDict

    clusterDict :> ClusterData
         
