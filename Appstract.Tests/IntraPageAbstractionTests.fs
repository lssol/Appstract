module Appstract.Tests.IntraPageAbstractionTests

open System.Diagnostics
open System.Linq
open System.Collections.Generic

open NUnit.Framework
open FSharpPlus

open Appstract
open Appstract.Types
open Appstract.DOM
open Appstract.Tests.Common
open Appstract.Utils
open Appstract.Tests.Common.AutoOpenModule
open Appstract.IntraPageAbstraction

[<Test>]
let tagsTest () =
    let appstracter = Appstracter()
    let parentDict = computeParentsDict root
    let leaves = root.Nodes() |> Array.filter isLeaf
    let clustersMap = appstracter.ComputeClusters parentDict leaves

    appstracter.ComputeTags parentDict clustersMap leaves
    assertEqual 2 (appstracter.TagMap.[root] |> Dict.count)
    Assert.Pass()

[<Test>]
let boxExtractionTest () =
    let appstracter = Appstracter()
    let parentDict = computeParentsDict root
    let leaves = root.Nodes() |> Array.filter isLeaf
    let clusterMap = appstracter.ComputeClusters parentDict leaves
    appstracter.ComputeTags parentDict clusterMap leaves
    
//    let clusters = clusterMap |> Map.toArray |> Array.map snd |> Set.ofArray |> Set.toArray
//    let boxDict = clusters |> Array.map (appstracter.ExtractBoxes parentDict)
        
    Assert.Pass()

[<Test>]
let abstractionTest () =
    let result = appstract root
    assertEqual 6 (result.Nodes().Length)

[<Test>]
let abstractionOnSimpleRecursiveTree () =
    let html = getHtml "simple_recursive"
    let root = Node.FromString(html).Value
    let result = appstract root
    assertEqual 3 (result.Nodes().Length)

[<Test>]
let abstractionOnCats () =
    let html = getHtml "cat/pages/male"
    let root = Node.FromString(html).Value
    let watch = Stopwatch()
    watch.Start()
    let result = appstract root
    watch.Stop()
    printf "ellapsed time: %d" watch.ElapsedMilliseconds

[<Test>]
let abstractionOnHeavyWebsite () =
    let html = getHtml "amazon"
    let root = Node.FromString(html).Value
    let watch = Stopwatch()
    watch.Start()
    let result = appstract root
    watch.Stop()
    printf "ellapsed time: %d" watch.ElapsedMilliseconds
    
    