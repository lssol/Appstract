namespace Appstract.Front.Entities
{
    public class Page
    {
        public string Url { get; set; }
        public int NbNodes { get; set; }
        public int NbLinks { get; set; }
        public string ApplicationId  { get; set; }
        public string Content {get; set; }
        public string Origin {get; set; }
        public string Domain { get; set; }
    }
}