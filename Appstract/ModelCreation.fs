module Appstract.ModelCreation

open System.Collections.Generic
open Appstract.Types
open Appstract.DOM
open Appstract.Utils
open FSharp.Data
open FSharpPlus
open MBrace.FsPickler

type PageIdentificationData = { signature: string; id: string }

let createModel (requestPages: string seq) =
    let templates = requestPages |> Seq.map Node.FromString
    let templateIds =
        templates
        |> Seq.map (fun template -> template.Value.signature)
        |> Seq.zip requestPages
        |> Seq.map (fun (templateSignature, page) -> {|page = page; templateSignature = templateSignature|})
    let model =
        templates
        |> Seq.map (fun op -> op |> Option.get)
        |> Seq.map IntraPageAbstraction.appstract
        |> Seq.toList
        |> InterPageAbstraction.createModel
        
    model

let sourceToSignatureCouple (NodeId id) (source: HtmlNode) : (string * string) option =
    source.TryGetAttribute "signature"
    |> Option.map (fun attr -> (attr.Value(), id))

let nodeToSignatureCouples mapping (node: Node) =
    mapping
    |> Map.tryFind node
    |> Option.map
        (fun id ->
            node.abstractionData.source
            |> Seq.choose (sourceToSignatureCouple id)
            |> Seq.toList)
    |> Option.defaultValue List.empty

let getDuplicates seq =
    let mutable set = HashSet()
    let mutable duplicates = HashSet()

    seq |> Seq.iter (fun s ->
            if set.Contains(fst s)
            then duplicates.Add(fst s)
            else set.Add(fst s)
            |> ignore)
    
    duplicates
    
let identifyPage (model: AppModel) (src: string) =
    let model = model

    let (Template (page, mapping)) =
        src
        |> Node.FromString
        |> Option.get
        |> IntraPageAbstraction.appstract
        |> InterPageAbstraction.appstract model

    page.Nodes()
    |> Seq.collect (nodeToSignatureCouples mapping)
    |> Utils.Seq.removeDoubles fst //TODO understand why there are doubles in the first place
    |> Dict.ofSeq

let toSerializableTemplate (Template (node, mapping)) : TemplateSerializable =
    let newMapping =
        mapping
        |> Map.toList
        |> List.map (fun (node, (NodeId id)) -> (node.signature, id))
        |> Map.ofList

    TemplateSerializable(node, newMapping)

let fromSerializableTemplate (TemplateSerializable (node, mapping)) : Template =
    let signatureToNode =
        node.Nodes()
        |> Seq.map (fun node -> (node.signature, node))
        |> Map.ofSeq

    let newMapping =
        mapping
        |> Seq.map (|KeyValue|) // To convert a C# Dictionary
        |> Seq.map (fun (signature, nodeId) -> (signatureToNode.[signature], NodeId nodeId))
        |> Map.ofSeq

    Template(node, newMapping)

let toSerializableModel (model: AppModel) =
    let appTemplate =
        model.appTemplate |> toSerializableTemplate

    let templates =
        model.templates |> List.map toSerializableTemplate

    { appTemplate = appTemplate
      templates = templates }

let fromSerializableModel (model: AppModelSerializable) : AppModel =
    let appTemplate =
        model.appTemplate |> fromSerializableTemplate

    let templates =
        model.templates
        |> List.map fromSerializableTemplate

    { appTemplate = appTemplate
      templates = templates }

let serializeModel (model: AppModel) =
    let pickler = FsPickler.CreateBinarySerializer()
    pickler.Pickle(toSerializableModel model)

let unserializeModel (binaryModel: byte []) =
    let pickler = FsPickler.CreateBinarySerializer()

    pickler.UnPickle<AppModelSerializable>(binaryModel)
    |> fromSerializableModel
