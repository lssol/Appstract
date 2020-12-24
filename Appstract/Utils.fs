namespace Appstract.Utils

open System
open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices
open FuzzyString

module String =
    let LCS string1 string2 =
        ComparisonMetrics.LongestCommonSubsequence(string1, string2)
    let genId () = Guid.NewGuid().ToString()
module Map =
    let push key value = 
        let addToSeq = function
        | None -> value |> Some
        | Some l -> value |> Set.union l |> Some
        Map.change key addToSeq
        
    let findOrDefault def key table =
        (Map.tryFind key table)
        |> Option.defaultValue def
        
    let keys m =
        m
        |> Map.toSeq
        |> Seq.map fst
        |> Set.ofSeq

    let values m =
        m
        |> Map.toSeq
        |> Seq.map snd
    