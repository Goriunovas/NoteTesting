using System.Collections.Generic;
using System.Web.Http;
using Noted.Filters;
using Noted.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Noted.Controllers
{
    [JwtAuthentication]
    public class NotesController : ApiController
    {
        public MongoClient _client;
        public IMongoDatabase _db;

        public NotesController()
        {
            // MongoDb cluster username: Garetas
            //                  password: as519fewf123
            _client = new MongoClient("mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test");
            _db = _client.GetDatabase("NoteDB");
        }

        private MongoNote FindNoteById(List<MongoNote> Notes, int Id)
        {
            foreach (var note in Notes)
            {
                if (note.NoteId == Id)
                    return note;
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

        // GET: api/Categories
        public IHttpActionResult Get()
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
                return Ok(customUser.Notes);
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        // GET: api/Categories/5
        public IHttpActionResult Get(int id)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoNote MongoNote = FindNoteById(customUser.Notes, id);
                if (MongoNote == null)
                    return BadRequest("Note with Id " + id + " was not found.");
                return Ok(MongoNote);
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Note note)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoNote newNote = new MongoNote();

                newNote.NoteId = customUser.Notes[customUser.Notes.Capacity - 1].NoteId + 1;
                newNote.Name = note.Name;
                newNote.Text = note.Text;
                newNote.Categories = new List<MongoCategory>();
                newNote.Dates = new List<MongoDate>();

                foreach (var category in note.Categories)
                {
                    foreach (var userCategory in customUser.Categories)
                    {
                        if (category.CategoryId == userCategory.CategoryId)
                            newNote.Categories.Add(userCategory);
                    }
                }

                foreach (var date in note.Dates)
                {
                    newNote.Dates.Add(date);
                }

                customUser.Notes.Add(newNote);

                _db.GetCollection<MongoNote>("Notes").InsertOne(newNote);
                _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

                return CreatedAtRoute("DefaultApi", newNote.Id, newNote);
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }

        // PUT: api/Categories/5
        public IHttpActionResult Put(int id, [FromBody]Note note)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoNote oldNote = null;
                oldNote = FindNoteById(customUser.Notes, id);

                if (oldNote == null)
                    return BadRequest("Note with Id " + id + " was not found.");

                oldNote.Name = note.Name;
                oldNote.Text = note.Text;

                note.Categories.Clear();
                note.Dates.Clear();

                foreach (var category in note.Categories)
                {
                    foreach (var userCategory in customUser.Categories)
                    {
                        if (category.CategoryId == userCategory.CategoryId)
                            oldNote.Categories.Add(userCategory);
                    }
                }

                foreach (var date in note.Dates)
                {
                    oldNote.Dates.Add(date);
                }

                _db.GetCollection<MongoNote>("Notes").ReplaceOne(x => x.NoteId == id, oldNote);
                _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

                return Ok(oldNote);
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));




        }

        // DELETE: api/Categories/5
        public IHttpActionResult Delete(int id)
        {
            MongoCustomUser customUser = FindUserDocument();
            if (customUser != null)
            {
                MongoNote Note = null;
                Note = FindNoteById(customUser.Notes, id);

                if (Note == null)
                    return BadRequest("Note with Id " + id + " was not found.");

                customUser.Notes.Remove(Note);

                _db.GetCollection<MongoNote>("Notes").FindOneAndDelete(x => x.NoteId == id);
                _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

                return Ok();
            }
            return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
        }
    }
}
