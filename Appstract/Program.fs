// Learn more about F# at http://fsharp.org

open System
open System.Diagnostics
open System.IO
open Appstract
open Appstract.Types
open Appstract.DOM

let abstractionOnHeavyWebsite () =
    let html = File.ReadAllText "htmls/amazon.html"
    let root = Node.FromString(html).Value
    let watch = Stopwatch()
    watch.Start()
    let result = IntraPageAbstraction.appstract root
    watch.Stop()
    printf "ellapsed time: %d" watch.ElapsedMilliseconds
    
[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    abstractionOnHeavyWebsite ()
    0 // return an integer exit code
