// Learn more about F# at http://fsharp.org

open System
open System.Diagnostics
open System.IO
open Appstract
open Appstract.IntraPageAbstraction
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
    
let generateClusterData () =
    let html = File.ReadAllText "htmls/cat.html"
    let root = Node.FromString(html).Value
    let appstracter = Appstracter()
    let parentDict = computeParentsDict root
    let leaves = root.Nodes() |> List.filter isLeaf
    let clusterMap = appstracter.ComputeClusters parentDict leaves
    appstracter.ComputeTags parentDict clusterMap leaves
    
    let clusters = clusterMap |> Map.toList |> List.map snd |> Set.ofList |> Set.toList
    let boxDict = clusters |> List.map (appstracter.ExtractBoxes parentDict)
    ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    abstractionOnHeavyWebsite ()
    0 // return an integer exit code
