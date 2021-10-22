module Appstract.InterPageAbstraction

open System
open Appstract
open Appstract.Types
open Appstract.Utils
open Appstract.DOM
open FSharpPlus.Operators
open System.Linq

let pageToTemplate (templateId, page: Node) : Template =
    let mapping =
        page.Nodes()
        |> Seq.fold (fun map n -> map |> Map.add n (NodeId.Gen())) Map.empty
                     
    Template(templateId, page, mapping)
    
let getTemplateFromTemplates (templates: Template seq) : Template =
    printfn $"Creating the master template over the next {templates.Count()} matching"
    let reducer (Template (id1, template1, mapping)) (Template (id2, template2, _)) =
        let matching = Sftm.Match template1 template2
        let edges = matching.Edges |> Map.ofSeq
        
        let newMapping =
            mapping
            |> Map.filter (fun n _ -> edges |> Map.containsKey n)
            
        Template ("", template1, newMapping)
        
    templates |> Seq.reduce reducer

let updateMapping refMapping mapping matching =
    let updateId node originalId =
        matching
        |> Map.tryFind node
        |> Option.bind (fun refNode -> refMapping |> Map.tryFind refNode)
        |> Option.defaultValue originalId 
    
    mapping |> Map.map updateId
    
let identify (Template(_, refPage, refMapping)) (Template(id, page, mapping)) : Template =
    printfn $"Identifying a page containing {page.Nodes().Length} nodes"
    let matching = Sftm.Match page refPage
    let edges = matching.Edges |> Map.ofSeq
    let newMapping = updateMapping refMapping mapping edges
    // New Mapping contains
        
    Template(id, page, newMapping)

let matchToClosestTemplate (model:AppModel) (Template(_, page, mapping)) = 
    let templates = model.templates
    let bestId, bestMatching, templateMapping = 
        templates 
        |> Seq.map (fun (Template(id, p, m)) -> (id, (Sftm.Match page p), m))
        |> Seq.minBy (fun (_, matching, _) -> matching.Cost)
        
    printfn $"bestMatching has {bestMatching.NoMatch} no match"
    let newMapping = updateMapping templateMapping mapping (bestMatching.Edges |> Map.ofSeq)
    
    Template(bestId, page, newMapping)
    
let createModel : ModelCreator = fun pages ->
    printfn $"Creating a model from {pages.Length} pages"
    let templates = pages |> List.map pageToTemplate
    let appTemplate = getTemplateFromTemplates templates
    let templates = templates |> List.map (identify appTemplate)
    
    {appTemplate = appTemplate; templates = templates}
    
let appstract (model: AppModel) page =
    page
    |> pageToTemplate
    |> matchToClosestTemplate model
        

