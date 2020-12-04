module Appstract.Tests.Common
open Appstract.DOM
open Appstract.Types
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

let root = Node.FromString(htmlString).Value

[<AutoOpen>]
module AutoOpenModule =
    #if DEBUG
    let (|>) value func =
      let result = func value
      result
    #endif

