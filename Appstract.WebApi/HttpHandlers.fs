module Appstract.WebApi.HttpHandlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open Appstract.WebApi.Models

let bind<'a> = bindModel<'a> None

let abstractPage page =
    Successful.OK page
