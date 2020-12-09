namespace Appstract.Utils

open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices
open FuzzyString

module String =
    let LCS string1 string2 =
        ComparisonMetrics.LongestCommonSubsequence(string1, string2)
module Map =
    let push key value = 
        let addToSeq = function
        | None -> [value] |> Set.ofSeq |> Some
        | Some l -> [value] |> Set.ofSeq |> Set.union l |> Some
        Map.change key addToSeq
        
    let findOrDefault def key table =
        (Map.tryFind key table)
        |> Option.defaultValue def
        
    let keys m =
        m
        |> Map.toSeq
        |> Seq.map fst
        |> Set.ofSeq
    

module Settings =
    let MAX_LCS = 100
    let MIN_SIZE_CLUSTER = 3