module Appstract.WebApi.HttpHandlers

open Appstract
open FSharpPlus
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open System.Collections.Generic
open Appstract.WebApi.Models
open Appstract.DOM
open Appstract.Types
open Appstract.Utils
open FSharp.Data
open Appstract.WebApi.Services

let bind<'a> = bindModel<'a> None

let mutable models = Dictionary<string, AppModel>()

let abstractPage (page: Requests.WebPage)  =
    let root = Node.FromString(page.src).Value
    let abstractTree = IntraPageAbstraction.appstract root
    let nodes, edges = nodeToCyto abstractTree
    
    json {| edges = edges ; nodes = nodes |}

let createModel (requestPages: Requests.WebPages) =
    let model =
        requestPages.webpages
        |> Seq.choose (fun w -> Node.FromString(w.src))
        |> Seq.map (IntraPageAbstraction.appstract)
        |> InterPageAbstraction.createModel
        
    let modelId = String.genId()
    models.Add(modelId, model)
    
    json {| modelId = modelId |}

let sourceToSignatureCouple (NodeId(id)) (source: HtmlNode) : (string * string) option =
    source.TryGetAttribute "signature"
    |> Option.map (fun attr -> (attr.Value(), id))
    
let nodeToSignatureCouples mapping (node: Node) =
    let id = mapping |> Map.tryFind node
    let ids = id |> Option.map (fun id -> (Seq.choose (sourceToSignatureCouple id) (node.AbstractionData().Source)) |> Seq.toList)
    let result = ids |> Option.defaultValue List.empty
    
    result

let identifyPage (request: Requests.Identify) =
    let model = models.[request.modelId] ///TODO handle the case when model is null
    let (Template(page, mapping)) =
        request.src
        |> Node.FromString
        |> Option.defaultValue (Node.EmptyNode)
        |> IntraPageAbstraction.appstract
        |> InterPageAbstraction.appstract model
    
    let ids =
        page.Nodes()
        |> Seq.collect (nodeToSignatureCouples mapping)
        |> Seq.map (fun (signature, id) -> {| signature = signature; id = id |})
        |> Seq.toList
    
    json {| ids = ids |}
    