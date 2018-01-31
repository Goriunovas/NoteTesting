using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Noted.Models;
using MongoDB.Driver;
using Noted.Managers;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace Noted.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        public MongoClient _client;
        public IMongoDatabase _db;
        public PasswordManager passwordManager;

        public AccountController()
        {
            _client = new MongoClient("mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test");
            _db = _client.GetDatabase("NoteDB");
            passwordManager = new PasswordManager();
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public string Login([FromBody]UserLogin user)
        {
            List<MongoCustomUser> mongoUser = _db.GetCollection<MongoCustomUser>("Users").Find(x => x.Email == user.Email).ToList();

            if (mongoUser.Count != 0 && passwordManager.ValidatePassword(user.Password,mongoUser[0].Password,mongoUser[0].Salt))
                return JwtManager.GenerateToken(user.Email);

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register([FromBody]UserRegister user)
        {

            if (UserExists(user.Email))
                return BadRequest("This email is already in use");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            MongoCustomUser mongoUser = new MongoCustomUser();
            mongoUser.Email = user.Email;         

            mongoUser.Salt = passwordManager.GetRandomSalt();
            mongoUser.Password = passwordManager.HashPassword(user.Password, mongoUser.Salt);

            mongoUser.Categories = new List<ObjectId>();
            mongoUser.Tabs = new List<ObjectId>();
            mongoUser.Notes = new List<ObjectId>();

            Create(mongoUser);

            return Ok(JwtManager.GenerateToken(user.Email));
        }

        /*[AllowAnonymous]
        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword([FromBody]UserChangePassword user)
        {
            if (user.NewPassword != user.ConfirmPassword)
                return BadRequest("Password and confirmation password needs to be the same");

            ChangePassword(user.Email, user.NewPassword);

            return Ok();
        }

        public void ChangePassword(string Email, string NewPassword)
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
        }*/



        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }

        public bool UserExists(string Email)
        {
            if (Email == null || Email.Length == 0)
                return false;

            var collection = _db.GetCollection<MongoCustomUser>("Users");

            List<MongoCustomUser> mongoUser = _db.GetCollection<MongoCustomUser>("Users").Find(x => x.Email == Email).ToList();


            if (mongoUser.Count != 0)
                return true;

            return false;
        }

        public MongoCustomUser Create(MongoCustomUser p)
        {
            _db.GetCollection<MongoCustomUser>("Users").InsertOne(p);
            return p;
        }


    }
}
