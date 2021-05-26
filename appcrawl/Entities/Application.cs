using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace appcrawl.Entities
{
    public class Application
    {
        public Application(string name)
        {
            Name = name;
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get;                      set; }
        public string                Name      { get; set; }
        public IEnumerable<Template> Templates { get; set; } = new List<Template>();
        public byte[]?               Model     { get; set; }
    }
}