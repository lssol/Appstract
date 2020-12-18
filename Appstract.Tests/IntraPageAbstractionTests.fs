module Appstract.Tests.IntraPageAbstractionTests

open Appstract.Types
open Appstract.DOM
open NUnit.Framework
open Appstract.Tests.Common
open Appstract.Tests.Common.AutoOpenModule
open Appstract.IntraPageAbstraction
open System.Linq
open System.Collections.Generic

[<Test>]
let tagsTest () = 
    let parentDict = computeParentsDict root
    let leaves = root.Nodes() |> List.filter isLeaf
    let clustersMap = computeClusters parentDict leaves

    let tags = computeTags parentDict clustersMap leaves
    areEqual 2 (tags.[root] |> Map.count)
    Assert.Pass()

[<Test>]
let abstractionTest () =
    let result = intraPageAbstraction root
    areEqual 6 (result.Nodes().Length)

[<Test>]
let testShadowing() =
    let f number =
        let number = 2 * number
        number * 2
    
    let n = 5 
    printfn "%i" n
    
    let n = f n
    printfn "%i" n
    