using System.Collections.Generic;

namespace appcrawl.Models
{
    public class RenameApplicationModel
    {
        public string ApplicationId { get; set; }
        public string NewName { get; set; }
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
        public string ApplicationId { get; set; }
        public string Page { get; set; }
    }
    
    public class CreateModelViewModel
    {
        public string              applicationId;
        public IEnumerable<string> Pages { get; set; }
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
}