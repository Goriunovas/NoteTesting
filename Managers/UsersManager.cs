using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using Noted.Models;

namespace Noted.Managers
{
    public class UsersManager: DataManager
    {
        public bool CheckUser(string Email, string Password)
        {
            if (Email == null || Email.Length == 0)
                return false;

            var query = Query.EQ("Email", Email);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            if (mongoUser != null && mongoUser.Password == Password)
                return true;

            return false;
        }

        public bool UserExists(string Email)
        {
            if (Email == null || Email.Length == 0)
                return false;

            var query = Query.EQ("Email", Email);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            if (mongoUser != null)
                return true;

            return false;
        }

        public MongoCustomUser Create(MongoCustomUser p)
        {
            _db.GetCollection<MongoCustomUser>("Users").Save(p);
            return p;
        }

        public void ChangePassword(string Email ,string NewPassword)
        {
            if (Email == null || Email.Length == 0)
                return;

            var query = Query.EQ("Email", Email);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            if (mongoUser != null)
            {
                var update3 = Update<MongoCustomUser>.Set(p => p.Password, NewPassword);
                _db.GetCollection<MongoCustomUser>("Users").Update(query, update3);
            }
        }

    }
}