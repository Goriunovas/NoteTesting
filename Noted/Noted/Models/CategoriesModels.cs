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

    public class MongoCategory
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("CategoryId")]
        public int CategoryId { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}