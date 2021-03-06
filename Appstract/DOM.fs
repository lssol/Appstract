module Appstract.DOM

open FSharp.Data
open FSharp.Data.HtmlActivePatterns
open Appstract.Types
open System.Collections.Generic
open Appstract.Utils

type ParentDict = Dictionary<Node, Node option>

let ignoredTags = Set.ofList ["script"]
let ignoredAttributes = Set.ofList ["signature"]
let fromString html =
    let fromAttr = function | HtmlAttribute (name, value) -> (name, value)
    let fromAttrs = List.map fromAttr >> Map.ofList

    let rec fromNode node =
        match node with
        | HtmlElement (name, attributes, children) when not (ignoredTags.Contains(name)) ->
            Some { name = name
                   attributes = fromAttrs attributes
                   abstractionData = AbstractionData.Default node
                   children = children |> Array.ofList |> Array.choose fromNode}
        | _ -> None

    (HtmlDocument.Parse html).TryGetBody()
    |> Option.bind fromNode

let getNodes root =
    let result = List<Node>()
    let rec traverse (n: Node) =
        result.Add n
        n.children |> Array.iter traverse 

    traverse root
    result |> Seq.toArray

let computeParentsDict root =
    let parentDict = ParentDict()

    let rec traverse parent node =
        parentDict.Add(node, parent)
        node.children |> Array.iter (traverse (Some node))

    traverse None root
    parentDict

let isLeaf node = Array.isEmpty node.children

let computeRootFromLeafPath (parentDict: ParentDict) leaf =
    let rec compute node =
        match node with
        | None -> ""
        | Some n -> sprintf "{%s}/{%s}" (compute parentDict.[n]) n.name

    compute (Some leaf)

let actOnBranch (parentDict: ParentDict) (f: Node -> Node option -> int -> unit) node = 
    let rec act f node prev depth = 
        match node with
        | None -> ()
        | Some n -> 
            f n prev depth 
            act f parentDict.[n] prev (depth + 1)
    act f (Some node) None 0

type Node with
    static member FromString(s) = fromString s
    member this.ParentDict() = computeParentsDict this
    member this.Nodes() = getNodes this
    member this.ActOnBranch parentDict f = actOnBranch parentDict f this
