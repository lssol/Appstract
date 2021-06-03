using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace appcrawl.Entities
{
    public class Template
    {
        public Template(string applicationId, string name)
        {
            ApplicationId = applicationId;
            Name          = name;
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id            { get; set; }
        public string? Url           { get; set; }
        public string? Html          { get; set; }
        public string Name          { get; set; }
        public string  ApplicationId { get; set; }
        
        public IEnumerable<Element> Elements { get; set; } = new List<Element>();
    }
}