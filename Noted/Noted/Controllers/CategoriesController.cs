using System.Collections.Generic;
using System.Web.Http;
using Noted.Filters;
using Noted.Models;
using MongoDB.Driver;
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

        private MongoCategory FindCategoryById(List<MongoCategory> Categories, int Id)
        {
            foreach (var category in Categories)
            {
                if (category.CategoryId == Id)
                    return category;
            }
            return null;
        }

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
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
                return Ok(customUser.Categories);
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        [HttpGet]
        // GET: api/Categories/5
        public IHttpActionResult Get(int id)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoCategory mongoCategory = FindCategoryById(customUser.Categories, id);
                if (mongoCategory == null)
                    return BadRequest("Note with Id " + id + " was not found.");
                return Ok(mongoCategory);
            }     
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        [HttpPost]
        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Category category)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {

                MongoCategory newCategory = new MongoCategory();

                if (customUser.Categories.Count > 0)
                    newCategory.CategoryId = customUser.Categories[customUser.Categories.Count - 1].CategoryId + 1;
                else
                    newCategory.CategoryId = 0;
                newCategory.Name = category.Name;

                customUser.Categories.Add(newCategory);

                _db.GetCollection<MongoCategory>("Categories").InsertOne(newCategory);
                _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);
                
                return CreatedAtRoute("DefaultApi", newCategory.Id, newCategory);
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        [HttpPut]
        // PUT: api/Categories/5
        public IHttpActionResult Put(int id, [FromBody]Category category)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoCategory mongoCategory = FindCategoryById(customUser.Categories, id);

                if(mongoCategory == null)
                    return BadRequest("Note with Id " + id + " was not found.");

                mongoCategory.Name = category.Name;

                _db.GetCollection<MongoCategory>("Categories").ReplaceOne(x => x.CategoryId == id, mongoCategory);
                _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

                return Ok(mongoCategory);
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        [HttpDelete]
        // DELETE: api/Categories/5
        public IHttpActionResult Delete(int id)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoCategory mongoCategory = FindCategoryById(customUser.Categories, id);

                if (mongoCategory == null)
                    return BadRequest("Note with Id " + id + " was not found.");

                customUser.Categories.Remove(mongoCategory);

                _db.GetCollection<MongoCategory>("Categories").FindOneAndDelete(x => x.CategoryId == id);
                _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

                return Ok();
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }


    }
}
