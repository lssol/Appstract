module Appstract.VariabilityAnalysis

open Appstract
open Appstract.Types
open Appstract.DOM

let computeConsistency (node: Node) =
    let nodes = node.Nodes()
    let computeConsistency (n: Node) =
        let children = n.Children() |> List.map (fun child -> child.AbstractionData().Source.Count)
        if children.Length > 0 then
            let max = children |> List.max
            let sumConsistencies = children |> List.sumBy (fun l -> (double) l / (double) max) 
            sumConsistencies / (double) children.Length
        else 1.
        
    List.zip nodes (nodes |> List.map computeConsistency)
    |> Map.ofList