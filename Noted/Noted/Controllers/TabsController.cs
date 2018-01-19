using System.Collections.Generic;
using System.Web.Http;
using Noted.Filters;
using Noted.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Noted.Controllers
{
    [JwtAuthentication]
    public class TabsController : ApiController
    {
        public MongoClient _client;
        public MongoServer _server;
        public MongoDatabase _db;

        public TabsController()
        {
            _client = new MongoClient("mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test");
            _server = _client.GetServer();
            _db = _server.GetDatabase("NoteDB");
        }

        public MongoTab FindTabById(List<MongoTab> Tabs, int Id)
        {
            foreach (var tab in Tabs)
            {
                if (tab.TabId == Id)
                    return tab;
            }
            return null;
        }

        // GET: api/Categories
        public IEnumerable<MongoTab> Get()
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            return mongoUser.Tabs;
        }

        // GET: api/Categories/5
        public IHttpActionResult Get(int id)
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            MongoTab mongoTab = FindTabById(mongoUser.Tabs, id);
            if (mongoTab == null)
                return BadRequest("Tab with Id " + id + " was not found.");
            return Ok(mongoTab);
        }

        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Tab tab)
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);

            MongoTab newTab = new MongoTab();

            newTab.TabId = mongoUser.Tabs[mongoUser.Tabs.Capacity-1].TabId+1;
            newTab.Name = tab.Name;
            newTab.Categories = new List<MongoCategory>();

            foreach (var category in tab.Categories)
            {
                foreach (var userCategory in mongoUser.Categories)
                {
                    if (category.CategoryId == userCategory.CategoryId)
                        newTab.Categories.Add(userCategory);
                }
            }

            mongoUser.Tabs.Add(newTab);

            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return CreatedAtRoute("DefaultApi", newTab.Id, newTab);
        }

        // PUT: api/Categories/5
        public IHttpActionResult Put(int id, [FromBody]Tab tab)
        {
            MongoTab oldTab = null;
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            oldTab = FindTabById(mongoUser.Tabs, id);
            if (oldTab == null)
                return BadRequest("Note with Id " + id + " was not found.");

            oldTab.Name = tab.Name;
            oldTab.Categories.Clear();

            foreach (var category in tab.Categories)
            {
                foreach (var userCategory in mongoUser.Categories)
                {
                    if (category.CategoryId == userCategory.CategoryId)
                        oldTab.Categories.Add(userCategory);
                }
            }

            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return Ok(oldTab);
        }

        // DELETE: api/Categories/5
        public IHttpActionResult Delete(int id)
        {
            MongoTab tab = null;
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            tab = FindTabById(mongoUser.Tabs, id);

            if (tab == null)
                return BadRequest("Note with Id " + id + " was not found.");

            mongoUser.Tabs.Remove(tab);
            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return Ok();
        }
    }
}
