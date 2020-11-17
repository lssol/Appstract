namespace Appstract

open System.Collections.Generic

(*
    The objective of these types is to define the abstract model that will be inferred by Appstract.
    
    This abstraction must be able to model the following situations:
    - Records stuff
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
            
    
    My biggest enemy: the free text. Free text is a problem by essence because it breaks our first assumption which is:
    Content variations induce very few structural variations.
    Indeed, free text allows the users to create content containing several tags like span, a, strong, p...
    This greatly confuse our algorithms that essentially rely on structure invariance to abstract a page.
    
    One possible solution to this problem could be to just ignore these tags (common inline tags) and cut the tree when
    they are the last descendants.
*)

type Tag = Tag of string
type AttributeValue = AttributeValue of string
type AttributeName = AttributeName of string
type Attribute = Attribute of IDictionary<AttributeName, AttributeValue>

type Variation<'a> = 
type Element = { Tag: string; Attributes: Attribute seq; Content: string }
type Web
type PageModel = 

type AppModel = 