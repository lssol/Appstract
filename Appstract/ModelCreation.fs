module Appstract.ModelCreation

open System.Collections.Generic
open Appstract.Types
open Appstract.DOM
open Appstract.Utils
open FSharp.Data
open FSharpPlus
open MBrace.FsPickler

// template = Id * Content
let createModel (templates: (string * string) seq) =
    printfn "------------ CREATING A MODEL ------------"
    templates
    |> Seq.map (fun (id, content) -> (id, content |> Node.FromString))
    |> Seq.map (fun (id, node) -> (id, IntraPageAbstraction.appstract node)) 
    |> Seq.toList
    |> InterPageAbstraction.createModel

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
    
type identifyPageResultMappingEntry = {signature: string; id: string}
type identifyPageResult = {templateId: string; mapping: identifyPageResultMappingEntry seq}
let identifyPage (model: AppModel) (src: string) =
    printfn $"******* Identifying a page containing {src.Length} characters"
    let model = model

    let (Template (id, page, mapping)) =
        src
        |> Node.FromString
        |> Debug.print (fun n -> $"# nodes: {n.Nodes().Length}")
        |> IntraPageAbstraction.appstract
        |> Debug.print (fun n -> $"# nodes after intra: {n.Nodes().Length}")
        |> fun n -> ("", n)
        |> InterPageAbstraction.appstract model

    let finalMapping =
        page.Nodes()
        |> Seq.collect (nodeToSignatureCouples mapping)
        |> Utils.Seq.removeDoubles fst //TODO understand why there are doubles in the first place
        |> Seq.map (fun (signature, id) -> {signature = signature; id = id})
        
    let debug = finalMapping
                |> Seq.filter (fun e -> e.id = "5eb52893-0516-406a-9a52-d029d5ae966f")
                |> Seq.toList
    
    {templateId = id; mapping = finalMapping}

let toSerializableTemplate (Template (id, node, mapping)) : TemplateSerializable =
    let newMapping =
        mapping
        |> Map.toList
        |> List.map (fun (node, (NodeId nodeId)) -> (node.signature, nodeId))
        |> Map.ofList

    TemplateSerializable(id, node, newMapping)

let fromSerializableTemplate (TemplateSerializable (id, node, mapping)) : Template =
    let signatureToNode =
        node.Nodes()
        |> Seq.map (fun node -> (node.signature, node))
        |> Map.ofSeq

    let newMapping =
        mapping
        |> Seq.map (|KeyValue|) // To convert a C# Dictionary
        |> Seq.map (fun (signature, nodeId) -> (signatureToNode.[signature], NodeId nodeId))
        |> Map.ofSeq

    Template(id, node, newMapping)

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
