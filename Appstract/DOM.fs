module Appstract.DOM

open FSharp.Data
open FSharp.Data.HtmlActivePatterns
open Appstract.Types
open System.Collections.Generic

let ignoredTags = Set.ofList ["script"]
let ignoredAttributes = Set.ofList ["signature"]
let fromString html =
    let fromAttr =
        function
        | HtmlAttribute (name, value) -> (name, value)

    let fromAttrs = List.map fromAttr >> Map.ofList

    let rec fromNode node =
        match node with
        | HtmlElement (name, _, _) when ignoredTags.Contains(name) -> EmptyNode
        | HtmlElement (name, attributes, children) ->
            Element(name, fromAttrs attributes, AbstractionData.Default node, fromNodes children)
        | HtmlText content -> Text("", AbstractionData.Default node)
        | _ -> EmptyNode

    and fromNodes =
        List.map fromNode
        >> List.filter (function
            | EmptyNode -> false
            | _ -> true)

    (HtmlDocument.Parse html).TryGetBody()
    |> Option.map fromNode

let getNodes root =
    let rec traverse (nodes: Node list) (nodesList: Node list) =
        match nodesList with
        | [] -> nodes
        | e :: rest ->
            match e with
            | Element (_, _, _, children) -> traverse (e :: nodes) (children @ rest)
            | _ -> traverse (e :: nodes) (rest)

    traverse [] [ root ]


let computeParentsDict root =
    let parentDict = Dictionary<Node, Node>()

    let rec traverse parent node =
        match node with
        | Element (_, _, _, children) ->
            parentDict.Add(node, parent)
            children |> List.iter (traverse node)
        | _ -> parentDict.Add(node, parent)

    traverse EmptyNode root
    parentDict

let isLeaf =
    function
    | Element (_, _, _, children) when not children.IsEmpty -> false
    | _ -> true

let computeRootFromLeafPath (parentDict: IDictionary<Node, Node>) leaf =
    let rec compute node =
        match node with
        | EmptyNode -> ""
        | Element (name, _, _, _) -> sprintf "{%s}/{%s}" (compute parentDict.[node]) name
        | Text (_) -> sprintf "{%s}/TEXT" (compute parentDict.[node])

    compute leaf

type Node with
    static member FromString(s) = fromString s
    member this.ParentDict() = computeParentsDict this
    member this.Nodes() = getNodes this
    member this.Children() = 
        match this with
        | Element(_, _, _, children) -> children
        | _ -> List.empty
