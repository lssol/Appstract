using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Appstract.Front.Entities
{
    public class Page
    {
        [Editable(false)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Url { get; set; }
        public int NbNodes { get; set; }
        public int NbLinks { get; set; }
        public string ApplicationId  { get; set; }
        public string Content {get; set; }
        public string Origin {get; set; }
        public string Domain { get; set; }
    }
}