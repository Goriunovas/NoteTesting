using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Noted.Models
{
    public class Tab
    {
        public string Name { get; set; }
        public List<MongoCategory> Categories { get; set; }
    }

    public class MongoTab
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("TabId")]
        public int TabId { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Categories")]
        public List<MongoCategory> Categories { get; set; }
    }
}