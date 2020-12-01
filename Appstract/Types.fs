namespace Appstract.Types

open System.Collections.Generic
open System

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
type Content = Content of string
type Tag = Tag of string
type AttributeValue = AttributeValue of string
type AttributeName = AttributeName of string
type Attribute = Attribute of AttributeName * AttributeValue
    
[<CustomEquality; CustomComparison>]
type Node =
    | Element of name:Tag * attributes:Attribute list * children:Node list
    | Text of content:Content
    | EmptyNode
    override this.Equals(obj) = Object.ReferenceEquals(this, obj)
    override this.GetHashCode() = this.GetHashCode()
    interface System.IComparable with
        member this.CompareTo obj = if this.Equals(obj) then 0 else 1

// Variability
type Id = Id of string
type Ids = IDictionary<Node, Id>

type Cluster = Cluster of Set<Node>

type IntraPageVariability = {
    Clusters: Cluster seq
    ClusterData: Map<Node, Cluster>
    BoxData: Map<Node, Set<Cluster>>
    OptionalData: HashSet<Node>
}
