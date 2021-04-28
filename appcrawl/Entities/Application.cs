using System.Collections;
using System.Collections.Generic;

namespace appcrawl.Entities
{
    public class Application
    {
        public Application(string name)
        {
            Name = name;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Template> Templates { get; set; } = new List<Template>();
    }
}