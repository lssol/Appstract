module Appstract.Tests.DOM

open Appstract.Types
open Appstract.DOM
open NUnit.Framework
open Appstract.Tests.Common
open Appstract.Tests.Common.AutoOpenModule
open Appstract.IntraPageAbstraction
open System.Linq
open System.Collections.Generic

[<Test>]
let htmlParsing () =
    root.Nodes() |> List.length |> areEqual 8

[<Test>]
let rootFromLeaf () =
    let getPath = computeRootFromLeafPath (root.ParentDict())
    let paths = 
        root.Nodes() 
        |> List.filter isLeaf
        |> List.map getPath
        |> List.iter (printfn "%s")
    Assert.Pass()

[<Test>]
let clusters () =
    let getPath = computeRootFromLeafPath (root.ParentDict())
    let clusterDict = computeClusters root
    let displayCluster (Cluster(nodes)) =
        printfn "----"
        nodes.ToList() |> Seq.map getPath |> Seq.iter (printfn "%s")
    clusterDict 
    |> Map.toSeq 
    |> Seq.map snd 
    |> Seq.iter displayCluster
    Assert.Pass()
    
