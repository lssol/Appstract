namespace appcrawl.Entities
{
    public class Template
    {
        public Template(string applicationId)
        {
            ApplicationId = applicationId;
        }

        public string? Id            { get; set; }
        public string? Url           { get; set; }
        public string? Html          { get; set; }
        public string? Name          { get; set; }
        public string  ApplicationId { get; set; }
    }
}