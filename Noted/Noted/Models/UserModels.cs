using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Noted.Models;

namespace Noted.Models
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegister
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserChangePassword
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tab> Tabs { get; set; }
        public List<Note> Notes { get; set; }
    }

    public class MongoCustomUser
    {
        public ObjectId Id { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("Categories")]
        public List<MongoCategory> Categories { get; set; }
        [BsonElement("Tabs")]
        public List<MongoTab> Tabs { get; set; }
        [BsonElement("Notes")]
        public List<MongoNote> Notes { get; set; }
    }
}