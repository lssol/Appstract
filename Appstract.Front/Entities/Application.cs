using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Appstract.Front.Entities
{
    public class Application
    {
        [Editable(false)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [Required]
        public string Domain { get; set; }
    }
}