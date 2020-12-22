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
    assertEqual 2 (tags.[root] |> Map.count)
    Assert.Pass()

[<Test>]
let abstractionTest () =
    let result = intraPageAbstraction root
    assertEqual 6 (result.Nodes().Length)

[<Test>]
let abstractionOnSimpleRecursiveTree () =
    let html = getHtml "simple_recursive"
    let root = Node.FromString(html).Value
    let result = intraPageAbstraction root
    assertEqual 6 (result.Nodes().Length)
