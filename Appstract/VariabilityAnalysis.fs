module Appstract.VariabilityAnalysis

open Appstract
open Appstract.Types
open Appstract.DOM

let computeConsistency (node: Node) =
    let nodes = node.Nodes()
    let computeConsistency (n: Node) =
        let children = n.children |> Array.map (fun child -> child.abstractionData.source.Count)
        if children.Length > 0 then
            let max = children |> Array.max
            let sumConsistencies = children |> Array.sumBy (fun l -> (double) l / (double) max) 
            sumConsistencies / (double) children.Length
        else 1.
        
    Array.zip nodes (nodes |> Array.map computeConsistency)
    |> Map.ofArray