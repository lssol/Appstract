module Appstract.Tests.InterPageAbstractionTests

open Appstract
open Appstract.Types
open Appstract.DOM
open NUnit.Framework
open Appstract.Tests.Common
open Appstract.Tests.Common.AutoOpenModule
open Appstract.IntraPageAbstraction
open System.Linq
open System.Collections.Generic

[<Test>]
let Sftm () =
    let matching = Sftm.Match root root |> Seq.toList
    matching |> Seq.iter (fun (n1, n2) -> assertEqual n1 n2)
    Assert.Pass()
    