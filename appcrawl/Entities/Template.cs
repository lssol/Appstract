namespace appcrawl.Entities
{
    public class Template
    {
        public Template(string html, string url)
        {
            Html = html;
            Url = url;
        }

        public string Id { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
    }
}