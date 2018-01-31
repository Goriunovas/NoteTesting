using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Noted.Models
{
    public class Category
    {
        public string Name { get; set; }
    }

    public class TempCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MongoCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}