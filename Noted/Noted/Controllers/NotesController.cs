using System.Collections.Generic;
using System.Web.Http;
using Noted.Filters;
using Noted.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

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

        private MongoNote FindNoteById(ObjectId Id)
        {
            List<MongoNote> mongoNote = _db.GetCollection<MongoNote>("Notes").Find(x => x.Id == Id).ToList();
            if (mongoNote.Count != 0)
                return mongoNote[0];
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            MongoCustomUser customUser = FindUserDocument();
            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));

            List<MongoNote> notes = new List<MongoNote>();
            foreach (var note in customUser.Notes)
            {
                notes.Add(FindNoteById(note));
            }
            return Ok(notes);

        }

        // GET: api/Categories/5
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ObjectId oId = ObjectId.Parse(id);
            MongoCustomUser customUser = FindUserDocument();
            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));

            MongoNote MongoNote = FindNoteById(oId);
            if (MongoNote == null)
                return BadRequest("Note with Id " + id + " was not found.");
            return Ok(MongoNote);


        }

        // POST: api/Categories
        public IHttpActionResult Post([FromBody]Note note)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            MongoCustomUser customUser = FindUserDocument();
            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));
            if (note == null)
                return BadRequest("Note was not found");

            MongoNote newNote = new MongoNote();

            newNote.Id = ObjectId.GenerateNewId();
            newNote.Name = note.Name;
            newNote.Text = note.Text;
            newNote.Categories = new List<ObjectId>();
            newNote.Dates = new List<MongoDate>();

            foreach (var category in note.Categories)
            {
                newNote.Categories.Add(ObjectId.Parse(category.Id));
            }

            foreach (var date in note.Dates)
            {
                newNote.Dates.Add(date);
            }

            customUser.Notes.Add(newNote.Id);

            _db.GetCollection<MongoNote>("Notes").InsertOne(newNote);
            _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

            return CreatedAtRoute("DefaultApi", newNote.Id, newNote);


        }

        // PUT: api/Categories/5
        public IHttpActionResult Put(string id, [FromBody]Note note)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ObjectId oId = ObjectId.Parse(id);
            MongoNote oldNote = null;
            oldNote = FindNoteById(oId);

            if (oldNote == null)
                return BadRequest("Note with Id " + id + " was not found.");

            oldNote.Name = note.Name;
            oldNote.Text = note.Text;

            oldNote.Categories.Clear();
            oldNote.Dates.Clear();

            foreach (var category in note.Categories)
            {
                oldNote.Categories.Add(ObjectId.Parse(category.Id));
            }

            foreach (var date in note.Dates)
            {
                oldNote.Dates.Add(date);
            }

            _db.GetCollection<MongoNote>("Notes").ReplaceOne(x => x.Id == oId, oldNote);

            return Ok(oldNote);
        }

        // DELETE: api/Categories/5
        public IHttpActionResult Delete(string id)
        {
            ObjectId oId = ObjectId.Parse(id);
            MongoCustomUser customUser = FindUserDocument();
            if (customUser == null)
                return InternalServerError(new System.Exception("Unexpected Error, User Document was not found"));

            MongoNote Note = null;
            Note = FindNoteById(oId);

            if (Note == null)
                return BadRequest("Note with Id " + id + " was not found.");

            customUser.Notes.Remove(oId);

            _db.GetCollection<MongoNote>("Notes").FindOneAndDelete(x => x.Id == oId);
            _db.GetCollection<MongoCustomUser>("Users").ReplaceOne(x => x.Email == RequestContext.Principal.Identity.Name, customUser);

            return Ok();


        }
    }
}
