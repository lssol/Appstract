module Appstract.Tests.WebApi

open System.Net.Mime
open Appstract.Tests
open Appstract.Tests.Common
open Appstract.WebApi
open Appstract.Types
open Appstract.DOM
open NUnit.Framework
open System.Text.Json
open System.Text.Json.Serialization
open Appstract.WebApi
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

let host = "http://localhost:14894/api/"

[<Test>]
let isUp () =
    let res = Http.Request(host + "health")
    assertEqual 200 res.StatusCode
    
[<Test>]
let getAbstractCytoTree () =
    let query = {| src = htmlString |}
    let queryJson = JsonSerializer.Serialize(query)
    
    let res = Http.Request(host + "intra",
                           body = TextRequest queryJson,
                           headers = [ContentType HttpContentTypes.Json])
    assertEqual 200 res.StatusCode
    
//[<Test>]
//let createModel () =
//    let templatesRequest =
//        templates
//        |> Seq.choose Node.FromString
//        |> Seq.map (fun f -> {| src = f |})
//    let query = {| src = htmlString |}
//    let queryJson = JsonSerializer.Serialize(query)
//    
//    let res = Http.Request(host + "intra",
//                           body = TextRequest queryJson,
//                           headers = [ContentType HttpContentTypes.Json])
//    assertEqual 200 res.StatusCode