module Appstract.Sftm

open Appstract
open Appstract.Types
open FSharpPlus.Operators
open tree_matching_csharp
open System.Collections.Generic
open Appstract.DOM

type Node = Appstract.Types.Node
type TM_Node = tree_matching_csharp.Node

let tokenizeAttributes (attrs: Attributes) : string list =
    attrs |> Map.toList |> List.collect (fun (key, value) -> [key; value])
    
let rec xPath (node: Node) (parent: Node Option) (partialXPath: string) =
    let findIndexAmongSiblings parent =
        parent.children
        |> Array.filter (fun n -> n.name = node.name)
        |> Array.tryFindIndex (fun n -> n = node)
        
    let index =
        parent
        >>= findIndexAmongSiblings
        |> function Some i -> sprintf "[%d]" i | None -> ""
    
    partialXPath + "/" + node.name + index

let NodeToTM_Node node =
    let mapping = Dictionary<TM_Node, Node>()
    let nodes = List<TM_Node>()
    let rec copy node parent tm_parent partialXPath =
        let newXPath = xPath node parent partialXPath
        let tokens = newXPath :: node.name :: (tokenizeAttributes node.attributes)
        let newNode = TM_Node(Value = List(tokens), Parent = tm_parent)
        nodes.Add(newNode)
        mapping.Add(newNode, node)
        node.children |> Array.iter (fun child -> copy child (Some node) newNode newXPath)

    copy node None null ""
    (nodes, mapping)
    
let Match : Matcher = fun node1 node2 ->
    printfn $"Matching two pages of sizes {node1.Nodes().Length} and {node2.Nodes().Length}"
    let matcher = SftmTreeMatcher(Settings.sftmParameters)
    let tm_node1, mapping1 = NodeToTM_Node node1
    let tm_node2, mapping2 = NodeToTM_Node node2

    let result = matcher.MatchTrees(tm_node1, tm_node2).Result
    let isNoMatch (e: Edge) = e.Source = null || e.Target = null
    let numberOfNoMatch = result.Edges |> Seq.filter isNoMatch |> Seq.length
    let edges =
        result.Edges
        |> Seq.filter (isNoMatch >> not)
        |> Seq.map (fun e -> (mapping1.[e.Source], mapping2.[e.Target]))
    
    printfn $"Finished matching the two pages with ${numberOfNoMatch} no match"
    {Edges = edges; Cost = result.Cost; NoMatch = numberOfNoMatch}
        

    