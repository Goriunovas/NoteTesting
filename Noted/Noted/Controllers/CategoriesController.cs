using System.Collections.Generic;
using System.Web.Http;
using Noted.Filters;
using Noted.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;

namespace Noted.Controllers
{
    [JwtAuthentication]
    public class CategoriesController : ApiController 
    {
        public MongoClient _client;
        public IMongoDatabase _db;         

        public CategoriesController()
        {
            _client = new MongoClient("mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test");
            _db = _client.GetDatabase("NoteDB");
        }
        [NonAction]
        private MongoCategory FindCategoryById(ObjectId Id)
        {
            List<MongoCategory> mongoCategory = _db.GetCollection<MongoCategory>("Categories").Find(x => x.Id == Id).ToList();
            if (mongoCategory.Count != 0)
                return mongoCategory[0];
            return null;
        }
        [NonAction]
        private MongoCustomUser FindUserDocument()
        {
            List<MongoCustomUser> mongoUser = _db.GetCollection<MongoCustomUser>("Users").Find(x => x.Email == RequestContext.Principal.Identity.Name).ToList();
            if (mongoUser.Count != 0)
                return mongoUser[0];
            return null;
        }

        [HttpGet]
        // GET: api/Categories
        public IHttpActionResult Get()
        {
            if (!ModelState.IsValid)
                return BadRequest();
            MongoCustomUser customUser = FindUserDocument();
            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));

            List<MongoCategory> categories = new List<MongoCategory>();
            foreach (var category in customUser.Categories)
            {
                categories.Add(FindCategoryById(category));
            }
            return Ok(categories);
        }

        [HttpGet]
        // GET: api/Categories/5
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            List<MongoCategory> mongoCategory = _db.GetCollection<MongoCategory>("Categories").Find(x => x.Id == ObjectId.Parse(id)).ToList();
            if (mongoCategory.Count != 0)
                return Ok(mongoCategory[0]);
            else
                return BadRequest("Category was not found");
        }

        [HttpPost]
        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.ToJson());

            MongoCustomUser customUser = FindUserDocument();

            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));

            MongoCategory newCategory = new MongoCategory();
            newCategory.Name = category.Name;
            newCategory.Id = ObjectId.GenerateNewId();
            _db.GetCollection<MongoCategory>("Categories").InsertOne(newCategory);
            
            customUser.Categories.Add(newCategory.Id);
            _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

            return CreatedAtRoute("DefaultApi", newCategory.Id, newCategory);

        }

        [HttpPut]
        // PUT: api/Categories/5
        public IHttpActionResult Put(string id, [FromBody]Category category)
        {
            ObjectId oId = ObjectId.Parse(id);
            MongoCategory mongoCategory = FindCategoryById(oId);

            if (mongoCategory == null)
                return BadRequest("Note with Id " + id + " was not found.");
            mongoCategory.Name = category.Name;
            _db.GetCollection<MongoCategory>("Categories").ReplaceOne(x => x.Id == oId, mongoCategory);

            return Ok(mongoCategory);
            
        }

        [HttpDelete]
        // DELETE: api/Categories/5
        public IHttpActionResult Delete(string id)
        {
            ObjectId oId = ObjectId.Parse(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.ToJson());

            MongoCustomUser customUser = FindUserDocument();

            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));

            MongoCategory mongoCategory = FindCategoryById(oId);

            if (mongoCategory == null)
                return BadRequest("Note with Id " + id + " was not found.");

            customUser.Categories.Remove(oId);

            _db.GetCollection<MongoCategory>("Categories").FindOneAndDelete(x => x.Id == oId);
            _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

            return Ok();
        }


    }
}
