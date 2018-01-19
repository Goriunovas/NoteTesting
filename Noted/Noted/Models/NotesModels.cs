using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Noted.Models
{
    public class Note
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public List<MongoCategory> Categories { get; set; }
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
        public ObjectId Id { get; set; }
        [BsonElement("TabId")]
        public int NoteId { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Text")]
        public string Text { get; set; }
        [BsonElement("Categories")]
        public List<MongoCategory> Categories { get; set; }
        [BsonElement("Categories")]
        public List<MongoDate> Dates { get; set; }
    }
}