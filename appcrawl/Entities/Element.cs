using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace appcrawl.Entities
{
    public class Element
    {
        public Element(string applicationId, string templateId, string name)
        {
            ApplicationId = applicationId;
            TemplateId = templateId;
            Name = name;
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ApplicationId { get; set; }
        public string Name { get; set; }
        public string ModelSignature { get; set; }
        public string TemplateId {get; set; }
    }
}