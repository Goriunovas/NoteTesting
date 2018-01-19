using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Noted.Models;
using Noted.Managers;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Noted.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        UsersManager usersManager = new UsersManager();

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public string Login([FromBody]UserLogin user)
        {
            if (usersManager.CheckUser(user.Email, user.Password))
            {
                return JwtManager.GenerateToken(user.Email);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register([FromBody]UserRegister user)
        {
            bool wat = usersManager.UserExists(user.Email);
            if (!wat)
                return BadRequest("This email is already in use");

            if(user.Password != user.ConfirmPassword)
                return BadRequest("Password and confirmation password needs to be the same");


            MongoCustomUser mongoUser = new MongoCustomUser();
            mongoUser.Email = user.Email;
            mongoUser.Password = user.Password;
            mongoUser.Categories = new List<MongoCategory>();
            mongoUser.Tabs = new List<MongoTab>();
            mongoUser.Notes = new List<MongoNote>();

            usersManager.Create(mongoUser);

            return Ok(JwtManager.GenerateToken(user.Email));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword([FromBody]UserChangePassword user)
        {
            if (user.NewPassword != user.ConfirmPassword)
                return BadRequest("Password and confirmation password needs to be the same");

            usersManager.ChangePassword(user.Email, user.NewPassword);

            return Ok();
        }



        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }
    }
}
