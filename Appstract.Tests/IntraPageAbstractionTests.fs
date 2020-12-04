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
    let leaves = root.Nodes() |> Seq.filter isLeaf
    let clustersMap = computeClusters root

    let tags = computeTags parentDict clustersMap leaves
    areEqual 2 (tags.[root] |> Map.count)
    Assert.Pass()

