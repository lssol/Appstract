namespace Appstract.Types

open System.Collections.Generic
open System
open System.Runtime.CompilerServices
open Appstract.RelationalDatabase
open Appstract.Utils
open FSharp.Data

(*
    The objective of these types is to define the abstract model that will be inferred by Appstract.

    This abstraction must be able to model the following situations:
    - Intra-Page
        - An element is part of a record
        - An element is the maximal ancestor of a record
        - An element is the container of records
        - Template
            - A template must have placeholder ids. They refer to data that vary
        - Data
            - Pieces of information are called *instances*. They must be identified with
              - The id of the record it belongs to
              - The rank of the record in the container
              - The placeholder id of the record template
            - In addition to referencing a template id, an instance can reference another instance.
    - Inter-page abstraction
      - An element has a different content / tags / attributes between pages
      - An element does not appear in all pages


    My biggest enemy: the free text. Free text is a problem by essence because it breaks our first assumption which is:
    Content variations induce very few structural variations.
    Indeed, free text allows the users to create content containing several tags like span, a, strong, p...
    This greatly confuse our algorithms that essentially rely on structure invariance to abstract a page.

    One possible solution to this problem could be to just ignore these tags (common inline tags) and cut the tree when
    they are the last descendants.
*)


// DOM
type AbstractionType =
    | Normal
    | Optional
    | ZeroToMany
    | OneToMany
    with
    override this.ToString() =
        match this with
        | Normal -> "Normal"
        | Optional -> "Optional"
        | ZeroToMany -> "ZeroToMany"
        | OneToMany -> "OneToMany"

and AbstractionData =
    { AbstractionType: AbstractionType
      Source: Set<HtmlNode> }

    static member Default node =
        { AbstractionType = Normal
          Source = Set.singleton node }

type Attributes = Map<string, string>

[<CustomEquality; CustomComparison>]
type Node =
    | Element of name:string * Attributes * AbstractionData * children: Node list
    | Text of content: string * AbstractionData
    | EmptyNode
    override this.Equals(obj) = Object.ReferenceEquals(this, obj)
    override this.GetHashCode() = RuntimeHelpers.GetHashCode(this)

    member this.AbstractionData() =
        match this with
        | Element(_, _, data, _) -> data
        | Text(_, data) -> data
        | EmptyNode -> failwith "asked for the abstraction data of an empty node, it should never happen"
    static member AbstractionData (this: Node) = this.AbstractionData()
    
    member this.UpdateData f =
        match this with
        | Element(a, b, data, c) -> Element(a, b, f data, c)
        | Text(a, data) -> Text(a, f data)
        | EmptyNode -> failwith "asked to modify the abstraction data of an empty node, it should never happen"

    member this.UpdateAbstractionType f = 
        let updateAbstractionData data = {data with AbstractionType = (f data.AbstractionType)}
        this.UpdateData updateAbstractionData

    member this.Children(): Node list =
        match this with
        | Element(_, _, _, children) -> children
        | _ -> List.empty
    static member Children (node: Node) = node.Children()
        
    member this.Name() =
        match this with
        | Element(tag, _, _, _) -> tag
        | Text _ -> "TEXT"
        | EmptyNode -> failwith "asked for tag of emptynode"

    interface IComparable with
        member this.CompareTo obj = this.GetHashCode() - obj.GetHashCode()

type Cluster = Cluster of Set<Node>

// Tree Matching
type MatcherResult = {
    Edges: (Node * Node) seq
    Cost: double
}
type Matcher = Node -> Node -> MatcherResult

// INTER
type NodeId = NodeId of string with
    static member Gen () = NodeId (String.genId())
type Template = Template of Node * Map<Node, NodeId>

type AppModel = { appTemplate: Template; templates: Template seq }
    
type ModelCreator = Node seq -> AppModel
type InformationExtractor = AppModel -> RelDb -> Node -> RelDb