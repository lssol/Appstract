namespace Appstract.DOM

open FSharp.Data
open FSharp.Data.HtmlActivePatterns
open Appstract.Types

module Parse =
    let fromString html =
        let fromAttr = function
            | HtmlAttribute (name, value) -> Attribute(AttributeName(name), AttributeValue(value))
        let fromAttrs = List.map fromAttr
        let rec fromNode = function
            | HtmlElement (name, attributes, children) -> Element(Tag(name), fromAttrs attributes, fromNodes children)
            | HtmlText content -> Text(Content(content))
            | _ -> EmptyNode
        and fromNodes =
            List.map fromNode
            >> List.filter (function EmptyNode -> false | _ -> true)
        
        (HtmlDocument.Parse html)
            .TryGetBody()
            |> Option.map fromNode

module Utils =
    let getNodes root =
        let rec traverse (nodes: Node list) (nodesList: Node list) =
            match nodesList with
            | [] -> nodes
            | e::rest ->
                match e with
                | Element(_, _, children) ->
                    traverse (e :: nodes) (children @ rest)
                | _ -> traverse (e :: nodes) (rest)
                
        traverse [] [root]

    type Node with
        member this.GetNodes() = getNodes this
