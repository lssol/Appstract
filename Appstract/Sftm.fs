module Appstract.Sftm

open Appstract
open Appstract.Types
open FSharpPlus.Operators
open tree_matching_csharp
open System.Collections.Generic

type Node = Appstract.Types.Node
type TM_Node = tree_matching_csharp.Node

let tokenizeAttributes (attrs: Attributes) : string list =
    attrs |> Map.toList |> List.collect (fun (key, value) -> [key; value])
    
let rec xPath (el: Node) (parent: Node Option) (partialXPath: string) =
    let findIndexAmongChildren =
        Node.Children
        >> List.filter (fun n -> n.Name() = el.Name())
        >> Seq.tryFindIndex (fun n -> n = el)
        
    let index = parent >>= findIndexAmongChildren
    
    partialXPath + "/" + el.Name() + (match index with | Some i -> sprintf "[%d]" i | None -> "")

let NodeToTM_Node node =
    let mapping = Dictionary<TM_Node, Node>()
    let nodes = List<TM_Node>()
    let rec copy node parent tm_parent partialXPath =
        match node with
        | Element (name, attributes, _, children) ->
            let newXPath = xPath node parent partialXPath
            let tokens = newXPath :: name :: (tokenizeAttributes attributes)
            let newNode = TM_Node(Value = List(tokens), Parent = tm_parent)
            nodes.Add(newNode)
            mapping.Add(newNode, node)
            children |> List.iter (fun child -> copy child (Some node) newNode newXPath)
        | _ -> ()

    copy node None null ""
    (nodes, mapping)
    
let Match : Matcher = fun node1 node2 ->
    let matcher = SftmTreeMatcher(Settings.sftmParameters)
    let tm_node1, mapping1 = NodeToTM_Node node1
    let tm_node2, mapping2 = NodeToTM_Node node2

    let result = matcher.MatchTrees(tm_node1, tm_node2).Result
    result.Edges
    |> Seq.filter (fun e -> e.Source <> null && e.Target <> null)
    |> Seq.map (fun e -> (mapping1.[e.Source], mapping2.[e.Target]))

    