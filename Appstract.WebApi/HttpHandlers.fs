module Appstract.WebApi.HttpHandlers

open Appstract
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open Appstract.WebApi.Models
open Appstract.DOM
open Appstract.Types
open Appstract.IntraPageAbstraction
open Appstract.WebApi.Services

let bind<'a> = bindModel<'a> None

let abstractPage (page: Requests.WebPage)  =
    let root = Node.FromString(page.src).Value
    let abstractTree = intraPageAbstraction root
    let nodes, edges = nodeToCyto abstractTree
    
    json {| edges = edges ; nodes = nodes |}
