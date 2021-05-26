module Appstract.ModelCreation

open System
open Appstract.Types
open Appstract.DOM
open Appstract.Utils
open FSharp.Data
open MBrace.FsPickler

type PageIdentificationData = { signature: string; id: string }

let createModel (requestPages: string seq) =
    requestPages
    |> Seq.choose Node.FromString
    |> Seq.map IntraPageAbstraction.appstract
    |> Seq.toList
    |> InterPageAbstraction.createModel

let sourceToSignatureCouple (NodeId(id)) (source: HtmlNode) : (string * string) option =
    source.TryGetAttribute "signature"
    |> Option.map (fun attr -> (attr.Value(), id))
    
let nodeToSignatureCouples mapping (node: Node) =
    let id = mapping |> Map.tryFind node
    let ids = id |> Option.map (fun id ->
        node.abstractionData.source
        |> Seq.choose (sourceToSignatureCouple id)
        |> Seq.toList)
    let result = ids |> Option.defaultValue List.empty
    
    result

let identifyPage (model: AppModel) (src: string) =
    let model = model
    
    let (Template(page, mapping)) =
        src
        |> Node.FromString
        |> Option.get
        |> IntraPageAbstraction.appstract
        |> InterPageAbstraction.appstract model
        
    page.Nodes()
    |> Seq.collect (nodeToSignatureCouples mapping)
    |> Seq.map (fun (signature, id) -> { signature = signature; id = id })
    |> Seq.toList

let serializeModel (model: AppModel) =
    let pickler = FsPickler.CreateBinarySerializer()
    pickler.Pickle model
    
let unserializeModel (binaryModel: byte[]) =
    let pickler = FsPickler.CreateBinarySerializer()
    pickler.UnPickle<AppModel>(binaryModel)
