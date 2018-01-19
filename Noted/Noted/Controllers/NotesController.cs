using System.Collections.Generic;
using System.Web.Http;
using Noted.Filters;
using Noted.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Noted.Controllers
{
    public class NotesController : ApiController
    {
        public MongoClient _client;
        public MongoServer _server;
        public MongoDatabase _db;

        public NotesController()
        {
            // MongoDb cluster username: Garetas
            //                  password: as519fewf123
            _client = new MongoClient("mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test");
            _server = _client.GetServer();
            _db = _server.GetDatabase("NoteDB");
        }

        public MongoNote FindNoteById(List<MongoNote> Notes, int Id)
        {
            foreach (var note in Notes)
            {
                if (note.NoteId == Id)
                    return note;
            }
            return null;
        }

        // GET: api/Categories
        public IEnumerable<MongoNote> Get()
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            return mongoUser.Notes;
        }

        // GET: api/Categories/5
        public IHttpActionResult Get(int id)
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            MongoNote MongoNote = FindNoteById(mongoUser.Notes, id);
            if (MongoNote == null)
                return BadRequest("Note with Id " + id + " was not found.");
            return Ok(MongoNote);
        }

        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Note note)
        {
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);

            MongoNote newNote = new MongoNote();

            newNote.NoteId = mongoUser.Tabs[mongoUser.Tabs.Capacity - 1].TabId + 1;
            newNote.Name = note.Name;
            newNote.Text = note.Text;
            newNote.Categories = new List<MongoCategory>();
            newNote.Dates = new List<MongoDate>();

            foreach (var category in note.Categories)
            {
                foreach (var userCategory in mongoUser.Categories)
                {
                    if (category.CategoryId == userCategory.CategoryId)
                        newNote.Categories.Add(userCategory);
                }
            }

            foreach (var date in note.Dates)
            {
                newNote.Dates.Add(date);
            }


            mongoUser.Notes.Add(newNote);

            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return CreatedAtRoute("DefaultApi", newNote.Id, newNote);
        }

        // PUT: api/Categories/5
        public IHttpActionResult Put(int id, [FromBody]Note note)
        {
            MongoNote oldNote = null;
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);

            oldNote = FindNoteById(mongoUser.Notes, id);

            if (oldNote == null)
                return BadRequest("Note with Id " + id + " was not found.");

            oldNote.Name = note.Name;
            oldNote.Text = note.Text;

            note.Categories.Clear();
            note.Dates.Clear();

            foreach (var category in note.Categories)
            {
                foreach (var userCategory in mongoUser.Categories)
                {
                    if (category.CategoryId == userCategory.CategoryId)
                        oldNote.Categories.Add(userCategory);
                }
            }

            foreach (var date in note.Dates)
            {
                oldNote.Dates.Add(date);
            }

            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return Ok(oldNote);
        }

        // DELETE: api/Categories/5
        public IHttpActionResult Delete(int id)
        {
            MongoNote Note = null;
            var query = Query.EQ("Email", RequestContext.Principal.Identity.Name);
            MongoCustomUser mongoUser = _db.GetCollection<MongoCustomUser>("Users").FindOne(query);
            Note = FindNoteById(mongoUser.Notes, id);

            if (Note == null)
                return BadRequest("Note with Id " + id + " was not found.");

            mongoUser.Notes.Remove(Note);
            _db.GetCollection<MongoCustomUser>("Users").Save(mongoUser);

            return Ok();
        }
    }
}
