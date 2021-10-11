using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Appstract.Front.Entities
{
    public class ClusterEntry
    {
        public string Url { get; set; }
        public double Silhouette { get; set; }
    }

    public class Clustering
    {
        [Editable(false)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public double Silhouette { get; set; }
        public string PagesHash { get; set; }
        public string ApplicationId { get; set; }
        public string Domain { get; set; }
        public int NbClusters { get; set; }
        public IEnumerable<IEnumerable<ClusterEntry>> Clusters { get; set; }
        public bool Outdated { get; set; }
        public string Clusterer { get; set; }
    }
}