﻿using System.Collections;
using System.Collections.Generic;

namespace appcrawl.Models
{
    public class RenameApplicationModel
    {
        public string ApplicationId { get; set; }
        public string NewName { get; set; }
    }
    
    public class SetHostApplicationModel
    {
        public string ApplicationId { get; set; } 
        public string Host          { get; set; }
    }
        
    public class CreateTemplateModel
    {
        public string ApplicationId { get; set; }
    }

    public class RenameTemplateModel
    {
        public string TemplateId { get; set; }
        public string NewName { get; set; }
    }

    public class IdentifyPageModel
    {
        public string Host { get; set; }
        public string Page { get; set; }
    }

    public class TemplateViewModel
    {
        public string Content    { get; set; }
        public string TemplateId { get; set; }
    }
    
    public class CreateModelViewModel
    {
        public string              ApplicationId { get; set; }
        public IEnumerable<TemplateViewModel> Templates         { get; set; }
    }
    
    public class RemoveTemplateModel
    {
        public string TemplateId { get; set; }
    }

    public class RemoveApplicationModel
    {
        public string ApplicationId { get; set; }
    }
    
    public class SetUrlTemplateModel
    {
        public string TemplateId { get; set; }
        public string Url { get; set; }
    }
    
    public class RemoveElementModel
    {
        public string ElementId { get; set; }
    }

    public class CreateElementModel
    {
        public string ApplicationId { get; set; }
        public string TemplateId { get; set; }
    }
    
    public class RenameElementModel
    {
        public string ElementId { get; set; }
        public string Name { get; set; }
    }
    
    public class UpdateSignatureElementModel
    {
        public string ElementId { get; set; }
        public string Signature { get; set; }
    }

    public class IdentifyResultModel
    {
        public class Element
        {
            public string Label { get; set; }
            public string Id { get; set; }
        }

        public class MappingEntry
        {
            public string Signature { get; set; }
            public string Id { get; set; }
        }
        
        public string TemplateId { get; set; }
        public string TemplateUrl { get; set; }
        public IEnumerable<Element> Elements { get; set; }
        public IEnumerable<MappingEntry> Mapping { get; set; }
    }
}