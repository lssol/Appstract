module Appstract.WebApi.Services

open System
open Appstract.Types
open Appstract.DOM
open Appstract.Utils
open FSharpPlus.Operators
open FSharpPlus
open FSharp.Data

type AdditionalInfo = string * Map<Node, string>

let nodeToCyto (node: Node) (additionalInfo: AdditionalInfo list) =
    let parentDict = node.ParentDict()
    let nodes = node.Nodes()
    
    let nodeIds = nodes |> Seq.fold (fun map n -> map |> Map.add n (Guid.NewGuid())) Map.empty
        
    let nodeToEdge node =
        let parentId = nodeIds |> Map.tryFind parentDict.[node]
        let nodeId = nodeIds |> Map.tryFind node
        match parentId, nodeId with
        | (Some p, Some n) ->
            Some {| data = {| source = p; target = n; abstractionType = node.AbstractionData().AbstractionType.ToString() |} |}
        | _ -> None
          
    let additionalInfoTuple node (label, map) =
        map
        |> Map.tryFind node
        |> Option.map (fun info -> (label, info))
        
    let nodeToCyto (node: Node) =
        let sourceSignatures =
            node.AbstractionData().Source
            |> Seq.choose (fun source -> source.TryGetAttribute "signature")
            |> Seq.map (fun attr -> attr.Value())
        
        let infos =
            additionalInfo
            |> List.choose (additionalInfoTuple node)
            |> Map.ofList
            
        match nodeIds |> Map.tryFind node with
        | Some n -> Some {| data = {| id = n; sourceSignatures = sourceSignatures; additionalInfo = infos |} |}
        | None -> None

    let cytoEdges = nodes |> Seq.choose nodeToEdge 
    let cytoNodes = nodes |> Seq.choose nodeToCyto
    
    (cytoNodes, cytoEdges)