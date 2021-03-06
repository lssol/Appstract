module Appstract.WebApi.HttpHandlers

open Appstract
open FSharpPlus
open FSharpPlus.Control
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
    
    let consistencies =
        VariabilityAnalysis.computeConsistency abstractTree
        |> Map.map (fun _ v -> v.ToString())
        
    let nodes, edges = nodeToCyto abstractTree [("consistency", consistencies)]
    
    json {| edges = edges ; nodes = nodes; |}

let createModel (requestPages: Requests.WebPages) =
    let model =
        requestPages.webpages
        |> List.choose (fun w -> Node.FromString(w.src))
        |> List.map (IntraPageAbstraction.appstract)
        |> InterPageAbstraction.createModel
        
    let modelId = String.genId()
    models.Add(modelId, model)
    
    json {| modelId = modelId |}

let sourceToSignatureCouple (NodeId(id)) (source: HtmlNode) : (string * string) option =
    source.TryGetAttribute "signature"
    |> Option.map (fun attr -> (attr.Value(), id))
    
let nodeToSignatureCouples mapping (node: Node) =
    let id = mapping |> Map.tryFind node
    let ids = id |> Option.map (fun id -> (Seq.choose (sourceToSignatureCouple id) (node.abstractionData.source)) |> Seq.toList)
    let result = ids |> Option.defaultValue List.empty
    
    result

let identifyPage (request: Requests.Identify) =
    let model = models.[request.modelId] ///TODO handle the case when model is null
    
    let (Template(page, mapping)) =
        request.src
        |> Node.FromString
        |> Option.get
        |> IntraPageAbstraction.appstract
        |> InterPageAbstraction.appstract model
        
    let ids =
        page.Nodes()
        |> Seq.collect (nodeToSignatureCouples mapping)
        |> Seq.map (fun (signature, id) -> {| signature = signature; id = id |})
        |> Seq.toList
    
    json {| ids = ids |}
    