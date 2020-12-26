module Appstract.Tests.InterPageAbstractionTests

open System.IO
open Appstract
open Appstract.Types
open NUnit.Framework
open Appstract.Tests.Common
open Appstract.Tests.Common.AutoOpenModule
open System.Linq
open System.Collections.Generic

[<Test>]
let Sftm () =
    let matching = Sftm.Match root root |> Seq.toList
    matching |> Seq.iter (fun (n1, n2) -> assertEqual n1 n2)
    Assert.Pass()
    
[<Test>]
let createModel () =
    let read path = File.ReadAllText path
    let model =
        ["htmls/cat/templates/male.html"; "htmls/cat/templates/sandra.html"]
        |> Seq.choose (read >> DOM.fromString >> (Option.map IntraPageAbstraction.intraPageAbstraction))
        |> InterPageAbstraction.createModel
    
    Assert.Pass()
