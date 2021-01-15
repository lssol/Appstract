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
let toTM_Node () =
     let tm_nodes = Sftm.NodeToTM_Node root
     Assert.Pass()
    
[<Test>]
let sftm () =
    let matching = (Sftm.Match root root).Edges |> Seq.toList
    matching |> Seq.iter (fun (n1, n2) -> assertEqual n1 n2)
    Assert.Pass()
    
let model () =
    templates
    |> List.choose (read >> DOM.fromString >> (Option.map IntraPageAbstraction.appstract))
    |> InterPageAbstraction.createModel
        
[<Test>]
let createModel () =
    let model = model()
    Assert.Pass()
    
[<Test>]
let appstract () =
    let model = model()
    let appstractedPages =
        pages
        |> Seq.choose (read >> DOM.fromString >> (Option.map IntraPageAbstraction.appstract))
        |> Seq.map (InterPageAbstraction.appstract model)
        |> Seq.toList
        
    Assert.Pass()
