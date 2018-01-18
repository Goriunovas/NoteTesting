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
        public MongoServer _server;
        public MongoDatabase _db;         

        public CategoriesController()
        {
            _client = new MongoClient("mongodb://Garetas:<9SF3dHKDcAVCfHq2>@notetesting-shard-00-00-msig9.mongodb.net:27017,notetesting-shard-00-01-msig9.mongodb.net:27017,notetesting-shard-00-02-msig9.mongodb.net:27017/admin?replicaSet=NoteTesting-shard-0&ssl=true");
            _server = _client.GetServer();
            _db = _server.GetDatabase("NotedDB");
        }

        public MongoCategory FindCategoryById(List<MongoCategory> Categories, int Id)
        {
            foreach (var category in Categories)
            {
                if (category.CategoryId == Id)
                    return category;
            }
            return null;
        }

        // GET: api/Categories
        public IEnumerable<MongoCategory> Get()
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            return mongoUser.Categories;
        }

        // GET: api/Categories/5
        public IHttpActionResult Get(int id)
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            MongoCategory mongoCategory = FindCategoryById(mongoUser.Categories, id);
            if (mongoCategory == null)
                return BadRequest("Note with Id " + id + " was not found.");
            return Ok(mongoCategory);
        }

        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Category category)
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);

            MongoCategory newCategory = new MongoCategory();
 
            newCategory.CategoryId = mongoUser.Categories[mongoUser.Categories.Capacity-1].CategoryId+1;
            newCategory.Name = category.Name;

            mongoUser.Categories.Add(newCategory);

            _db.GetCollection<MongoCustomUser>("Users").Save(newCategory);
            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return CreatedAtRoute("DefaultApi", newCategory.Id, newCategory);
        }

        // PUT: api/Categories/5
        public IHttpActionResult Put(int id, [FromBody]Category category)
        {
            MongoCategory oldCategory = null;
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            oldCategory = FindCategoryById(mongoUser.Categories, id);
            if(oldCategory == null)
                return BadRequest("Note with Id " + id + " was not found.");

            oldCategory.Name = category.Name;
            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return Ok(oldCategory);
        }

        // DELETE: api/Categories/5
        public IHttpActionResult Delete(int id)
        {
            MongoCategory Category = null;
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            Category = FindCategoryById(mongoUser.Categories, id);

            if(Category == null)
                return BadRequest("Note with Id " + id + " was not found.");

            mongoUser.Categories.Remove(Category);
            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return Ok();
        }


    }
}
