module Appstract.Sftm

open Appstract
open Appstract.Types
open FSharpPlus.Operators
open tree_matching_csharp
open System
open System.Collections.Generic

type Node = Appstract.Types.Node
type TM_Node = tree_matching_csharp.Node

let tokenizeAttributes (attrs: Attributes) : string list =
    attrs |> Map.toList |> List.collect (fun (key, value) -> [key; value])
    
let rec xPath (el: Node) (parent: Node Option) (partialXPath: string) =
    let findIndexAmongChildren = List.filter (fun (n:Node) -> n.Name() = el.Name()) >> Seq.tryFindIndex (fun n -> n = el)
    let index = parent >>= (fun p -> p.Children()) >>= findIndexAmongChildren
    
    partialXPath + "/" + el.Name() + (match index with | Some i -> $"[{i}]" | None -> "")

let NodeToTM_Node node =
    let mapping = Dictionary<TM_Node, Node>()
    let nodes = List<TM_Node>()
    let rec copy node parent tm_parent partialXPath =
        match node with
        | Element (name, attributes, data, children) ->
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
    let params: SftmTreeMatcher.Parameters =
        SftmTreeMatcher.Parameters(
            NoMatchCost = 4.3,
            MaxPenalizationChildren        = 0.4,
            MaxPenalizationParentsChildren = 0.2,
            LimitNeighbors = 100000,
            MetropolisParameters = Metropolis.Parameters(
                Gamma                   = 1f, // MUST be < 1
                Lambda                  = 2.5f,
                NbIterations            = 1,
                MetropolisNormalisation = true                  
            ),
            MaxTokenAppearance = (fun (n: int) -> sqrt (float n) |> int),
            PropagationParameters = SimilarityPropagation.Parameters(
                Envelop    = [|0.9; 0.1; 0.01|],
                Parent     = 0.4,
                Sibling    = 0.0,
                SiblingInv = 0.0,
                ParentInv  = 0.9,
                Children   = 0.0
            )
        )
        
    let matcher = SftmTreeMatcher(params)
    let 
    matcher.MatchTrees()
    