namespace Appstract.WebApi.Models

open Appstract.Types

module Requests =
    type WebPage = { src:string }
    type WebPages = { webpages: WebPage seq }
    type Identify = { src: string; modelId: string }

//module Responses =
//    type IdentifyResponse = 
