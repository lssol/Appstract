module Appstract.DOM

open System
open FSharp.Data
open FSharp.Data.HtmlActivePatterns
open Appstract.Types
open System.Collections.Generic
open Appstract.Utils

type ParentDict = Dictionary<Node, Node option>

let ignoredTags = Set.ofList ["script"]
let ignoredAttributes = Set.ofList ["signature"]
let fromString html =
    let fromAttrs attrs =
        attrs
        |> List.map (fun (HtmlAttribute(name, value)) -> (name, value))
        |> Map.ofList
        
    let getSignature attrs = attrs |> Map.tryFind "signature" |> Option.defaultValue ""
    
    let removeIgnoredAttributes (attrs: Map<string, string>) =
        attrs
        |> Map.filter (fun key _ -> not (ignoredAttributes.Contains(key)))
        
    let rec fromNode node =
        match node with
        | HtmlElement (name, attributes, children) when not (ignoredTags.Contains(name)) ->
            let attrs = fromAttrs attributes
            Some { signature = getSignature attrs
                   name = name
                   attributes = attrs |> removeIgnoredAttributes
                   abstractionData = AbstractionData.Default node
                   children = children |> Array.ofList |> Array.choose fromNode}
        | _ -> None

    let parsed = (HtmlDocument.Parse html)
    
    parsed.TryGetBody()
    |> Option.GetOrRaise (Exception($"Could not parse HTML: {html.Substring(0, 1000)}...{html.Substring(html.Length - 1000, 1000)}"))
    |> fromNode
    |> Option.GetOrRaise (Exception($"The HTML was parsed, but the obtained document parsed was not formed as expected"))

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
        | Some n -> sprintf "%s/%s" (compute parentDict.[n]) n.name

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
