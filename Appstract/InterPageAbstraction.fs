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
        let matching = Sftm.Match template1 template2 |> Map.ofSeq
        
        let newMapping =
            mapping
            |> Map.filter (fun n _ -> matching |> Map.containsKey n)
            
        Template (template1, newMapping)
        
    templates |> Seq.reduce reducer

let identify (Template(refPage, refMapping)) (Template(page, mapping)) : Template =
    let matching = Sftm.Match page refPage |> Map.ofSeq
    
    let updateId node originalId =
        matching
        |> Map.tryFind node
        |> Option.bind (fun refNode -> refMapping |> Map.tryFind refNode)
        |> Option.defaultValue originalId 
    
    let newMapping = mapping |> Map.map updateId
        
    Template(page, newMapping)

let createModel : ModelCreator = fun pages ->
    let templates = pages |> Seq.map pageToTemplate
    let appTemplate = getTemplateFromTemplates templates
    let templates = templates |> Seq.map (identify appTemplate)
    
    {appTemplate = appTemplate; templates = templates}
    



