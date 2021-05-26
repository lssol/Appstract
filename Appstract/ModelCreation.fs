namespace Appstract
open Appstract.Types
open Appstract.DOM
open Appstract.Utils
open FSharp.Data
open MBrace.FsPickler

type ModelCreation() =
    let createModel (requestPages: string list) =
        requestPages
        |> List.choose Node.FromString
        |> List.map IntraPageAbstraction.appstract
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

    let identifyPage (model: AppModel, src: string) =
        let model = model
        
        let (Template(page, mapping)) =
            src
            |> Node.FromString
            |> Option.get
            |> IntraPageAbstraction.appstract
            |> InterPageAbstraction.appstract model
            
        page.Nodes()
        |> Seq.collect (nodeToSignatureCouples mapping)
        |> Seq.map (fun (signature, id) -> {| signature = signature; id = id |})
        |> Seq.toList
        
    member this.createAndExportModel pages =
        let model = createModel pages
        let pickler = FsPickler.CreateBinarySerializer()
        pickler.Pickle(model)
        
    member this.importModel model = 