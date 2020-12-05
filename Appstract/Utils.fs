namespace Appstract.Utils

open System.Collections.Generic
open FSharp.Collections
open System.Runtime.CompilerServices

module Map =
    let push key value = 
        let addToSeq = function
        | None -> [value] |> Set.ofSeq |> Some
        | Some l -> [value] |> Set.ofSeq |> Set.union l |> Some
        Map.change key addToSeq