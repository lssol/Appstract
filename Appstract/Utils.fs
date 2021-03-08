namespace Appstract.Utils

open System
open System.Collections.Generic
open System.Linq
open FSharp.Collections
open System.Runtime.CompilerServices
open FuzzyString
open System.Linq
open FSharpPlus

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
    
module Dict =
    let tryFind<'a, 'b> (key: 'a) (dict: Dictionary<'a, 'b>) =
        match dict.ContainsKey(key) with
        | true -> Some dict.[key]
        | false -> None
    
    let change<'a, 'b> (key: 'a) (f: 'b option -> 'b) (dict: Dictionary<'a, 'b>) =
        let newValue =
            match dict.ContainsKey(key) with
            | true ->  f (Some dict.[key])
            | false -> f None
                
        dict.[key] <- newValue
    
    let getOrDefault<'a, 'b> (key: 'a) (def: 'b) dict =
        (tryFind key dict)
        |> Option.defaultValue def
        
    let incrementAt<'a> (key: 'a) (dict: Dictionary<'a, int>) =
        match tryFind key dict with
        | None -> dict.Add(key, 1)
        | Some n -> dict.[key] <- n + 1
    
    let map<'a, 'b, 'c> (f: 'a -> 'b -> 'c) (dict: Dictionary<'a, 'b>) : Dictionary<'a, 'c> =
        dict.Select(fun pair -> pair.Key, (f pair.Key pair.Value))
            .ToDictionary(fst, snd)
        
    let toSeq<'a, 'b> (dict: Dictionary<'a, 'b>) =
        dict.Select(fun pair -> (pair.Key, pair.Value))
    
    let toList<'a, 'b> (dict: Dictionary<'a, 'b>) =
        dict |> toSeq |> Seq.toList
        
    let toArray<'a, 'b> (dict: Dictionary<'a, 'b>) =
        dict |> toSeq |> Seq.toArray
        
    let ofSeq<'a, 'b> (sequence: ('a * 'b) seq) =
        sequence.ToDictionary(fst, snd)
    
    let exists<'a, 'b> f (dict: Dictionary<'a, 'b>) =
        dict.Any((fun pair -> (f pair.Key pair.Value)))
    
    let count<'a, 'b> (dict: Dictionary<'a, 'b>) =
        dict.Count


module HashSet =
    let Union s1 s2 =
        let result = HashSet()
        s1 |> Seq.iter (fun s -> result.Add(s) |> ignore)
        s2 |> Seq.iter (fun s -> result.Add(s) |> ignore)
        result
        
    let intersect<'a> (s1:'a seq) s2 =
        let result = HashSet()
        s2
        |> Seq.filter (fun s -> s1.Contains(s))
        |> Seq.iter (fun s -> result.Add(s) |> ignore)
        result
