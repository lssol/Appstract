module Appstract.DOM

open FSharp.Data
open FSharp.Data.HtmlActivePatterns
open Appstract.Types
open System.Collections.Generic

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

let computeParentsDict root =
    let parentDict = Dictionary<Node, Node>()
    let rec traverse parent node =
        match node with
        | Element(_, _, children) ->
            parentDict.Add(node, parent)
            children |> List.iter (traverse node) 
        | _ -> parentDict.Add(node, parent)
    traverse EmptyNode root
    parentDict
   
let isLeaf = function Element(_, _, children) when not children.IsEmpty -> false | _ -> true
    
let computeRootFromLeafPath (parentDict: IDictionary<Node, Node>) leaf =
    let rec compute node = 
        match node with 
        | EmptyNode -> ""
        | Element(Tag(name), _, _) -> $"{compute parentDict.[node]}/{name}"
        | Text(_) -> $"{compute parentDict.[node]}/TEXT"
        
    compute leaf

let rec cloneNode node = 
    let cloneAttr (Attribute(AttributeName(name), AttributeValue(value))) = Attribute(AttributeName(name), AttributeValue(value))
    match node with
    | EmptyNode -> EmptyNode
    | Text text -> Text text
    | Element(Tag(name), attrs, children) -> Element(Tag(name), List.map cloneAttr attrs, List.map cloneNode children)

type Node with
    static member FromString(s) = fromString s
    member this.ParentDict() = computeParentsDict this
    member this.Nodes() = getNodes this
    member this.Clone() = cloneNode this
