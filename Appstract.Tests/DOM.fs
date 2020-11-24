module Appstract.Tests

open Appstract.Types
open Appstract.DOM
open NUnit.Framework
open AutoOpenModule

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
    let root = Node.FromString(htmlString).Value
    root.Nodes() |> List.length |> areEqual 8

[<Test>]
let RootFromLeaf () =
    let root = Node.FromString(htmlString).Value
    let getPath = computeRootFromLeafPath (root.ParentDict())
    let paths = 
        root.Nodes() 
        |> List.filter isLeaf
        |> List.map getPath
        |> List.iter (printfn "%s")
    Assert.Pass()