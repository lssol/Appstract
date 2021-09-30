namespace Appstract.Types

open System.Collections.Generic
open System
open System.Runtime.CompilerServices
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
    { mutable abstractionType: AbstractionType
      mutable source: Set<HtmlNode> }

    static member Default node =
        { abstractionType = Normal
          source = Set.singleton node }

type Attributes = Map<string, string>

[<CustomEquality; CustomComparison>]
type Node =
    { signature: string
      name: string
      mutable attributes: Attributes
      abstractionData: AbstractionData
      mutable children: Node array }
    override this.Equals(obj) = Object.ReferenceEquals(this, obj)
    override this.GetHashCode() = RuntimeHelpers.GetHashCode(this)
    interface IComparable with
        member this.CompareTo obj = this.GetHashCode() - obj.GetHashCode()
type Cluster = Cluster of HashSet<Node>

// Tree Matching
type MatcherResult = {
    Edges: (Node * Node) seq
    Cost: double
    NoMatch: int
}
type Matcher = Node -> Node -> MatcherResult

// INTER
type NodeId = NodeId of string with
    static member Gen () = NodeId (String.genId())
type Template = Template of Node * Map<Node, NodeId>
type TemplateSerializable = TemplateSerializable of Node * Map<string, string>

type AppModel = { appTemplate: Template; templates: Template list }
type AppModelSerializable = { appTemplate: TemplateSerializable; templates: TemplateSerializable list }
    
type ModelCreator = Node list -> AppModel