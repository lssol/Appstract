module Appstract.InterPageAbstraction

open System
open Appstract
open Appstract.Types
open Appstract.Utils
open Appstract.DOM
open FSharpPlus.Operators

let pageToTemplate (page: Node) : Template =
    let mapping =
        page.Nodes()
        |> Seq.fold (fun map n -> map |> Map.add n (NodeId.Gen())) Map.empty
                     
    Template(page, mapping)
    
let getTemplateFromTemplates (templates: Template seq) : Template =
    let reducer (Template (template1, mapping)) (Template (template2, _)) =
        let matching = Sftm.Match template1 template2
        let edges = matching.Edges |> Map.ofSeq
        
        let newMapping =
            mapping
            |> Map.filter (fun n _ -> edges |> Map.containsKey n)
            
        Template (template1, newMapping)
        
    templates |> Seq.reduce reducer

let updateMapping refMapping mapping matching =
    let updateId node originalId =
        matching
        |> Map.tryFind node
        |> Option.bind (fun refNode -> refMapping |> Map.tryFind refNode)
        |> Option.defaultValue originalId 
    
    mapping |> Map.map updateId
    
let identify (Template(refPage, refMapping)) (Template(page, mapping)) : Template =
    let matching = Sftm.Match page refPage
    let edges = matching.Edges |> Map.ofSeq
    let newMapping = updateMapping refMapping mapping edges
        
    Template(page, newMapping)

let matchToClosestTemplate (model:AppModel) (Template(page, mapping)) = 
    let templates = model.templates
    let bestMatching, templateMapping = 
        templates 
        |> Seq.map (fun (Template(p, mapping)) -> ((Sftm.Match page p), mapping))
        |> Seq.minBy (fun (matching, _) -> matching.Cost)
    
    let newMapping = updateMapping templateMapping mapping (bestMatching.Edges |> Map.ofSeq)
    
    Template(page, newMapping)
    
let createModel : ModelCreator = fun pages ->
    let templates = pages |> List.map pageToTemplate
    let appTemplate = getTemplateFromTemplates templates
    let templates = templates |> List.map (identify appTemplate)
    
    {appTemplate = appTemplate; templates = templates}
    
let appstract (model: AppModel) page =
    page
    |> pageToTemplate
    |> matchToClosestTemplate model
        

