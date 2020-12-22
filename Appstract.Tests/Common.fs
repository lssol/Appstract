module Appstract.Tests.Common
open Appstract.DOM
open Appstract.Types
open NUnit.Framework
open System.IO

let assertEqual a b = Assert.AreEqual(a, b)
let fail () = Assert.Fail()

let htmlString = """
    <!DOCTYPE html>
    <html>
        <head></head>
        <body signature="body">
            <!-- A comment -->
            <h1 signature="h1">A title</h1>
            <div signature="div">
                <p signature="p1">p1</p>
                <p signature="p2">p2</p>
            </div>
        </body>
    </html>
"""

let root = Node.FromString(htmlString).Value

let getHtml name =
    let path = sprintf "htmls/%s.html" name
    File.ReadAllText path
    
[<AutoOpen>]
module AutoOpenModule =
    #if DEBUG
    let (|>) value func =
      let result = func value
      result
    #endif

