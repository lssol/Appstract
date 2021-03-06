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
    root.Nodes() |> Array.length |> assertEqual 8

[<Test>]
let rootFromLeaf () =
    let getPath = computeRootFromLeafPath (root.ParentDict())
    let paths = 
        root.Nodes() 
        |> Array.filter isLeaf
        |> Array.map getPath
        |> Array.iter (printfn "%s")
    Assert.Pass()

[<Test>]
let clusters () =
    let appstracter = Appstracter()
    let parentDict = root.ParentDict()
    let leaves = root.Nodes() |> Array.filter isLeaf
    let getPath = computeRootFromLeafPath parentDict 
    let clusterDict =  appstracter.ComputeClusters parentDict leaves
    let displayCluster (Cluster(nodes)) =
        printfn "----"
        nodes.ToList() |> Seq.map getPath |> Seq.iter (printfn "%s")
    clusterDict 
    |> Map.toSeq 
    |> Seq.map snd 
    |> Seq.iter displayCluster
    Assert.Pass()
    
