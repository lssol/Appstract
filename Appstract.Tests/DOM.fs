module Appstract.Tests

open Appstract.DOM
open Appstract.DOM.Utils
open NUnit.Framework

let areEqual a b = Assert.AreEqual(a, b)
let fail () = Assert.Fail()

let htmlString = """
    <!DOCTYPE html>
    <html>
        <head></head>
        <body>
            <!-- A comment -->
            <h1>A title</h1>
            <div>
                <p>p1</p>
                <p>p2</p>
            </div>
        </body>
    </html>
"""

[<Test>]
let HtmlParsing () =
    let root = Parse.fromString htmlString
        
    match root with
    | None -> fail ()
    | Some n -> n.GetNodes() |> List.length |> areEqual 8
    