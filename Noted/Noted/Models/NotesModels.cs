using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Noted.Models
{
    public class Note
    {        
        public string Name { get; set; }
        public string Text { get; set; }
        public List<TempCategory> Categories { get; set; }
        public List<MongoDate> Dates { get; set; }
    }

    public class MongoDate
    {
        [BsonElement("StartDate")]
        public string StartDate { get; set; }
        [BsonElement("EndDate")]
        public string EndDate { get; set; }
        [BsonElement("IsReminder")]
        public string IsReminder { get; set; }
    }
    
    public class MongoNote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Text")]
        public string Text { get; set; }
        [BsonElement("Categories")]
        public List<ObjectId> Categories { get; set; }
        [BsonElement("Dates")]
        public List<MongoDate> Dates { get; set; }
    }
}