namespace Appstract

open System.Collections.Generic

(*
    The objective of these types is to define the abstract model that will be infered by Appstract.
    
    This abstraction must be able to model the following situations:
    - Records stuff
        - An element is part of a record
        - An element is the maximal ancestor of a record
        - An element is the container of records
        - What is the fixed part of the record? (template)
        - What is the data associated with this record?
            - The data must be structured
            - Each information must be identified with the id of the record it belongs to and the rank
            
    
    My biggest enemy: the free text. Free text is a problem by essence because it breaks
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