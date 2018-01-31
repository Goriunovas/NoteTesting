using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Noted.Models;
using System.ComponentModel.DataAnnotations;

namespace Noted.Models
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegister
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserChangePassword
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tab> Tabs { get; set; }
        public List<Note> Notes { get; set; }
    }

    public class MongoCustomUser
    {
        public ObjectId Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("Salt")]
        public string Salt { get; set; }

        [BsonElement("Categories")]
        public List<ObjectId> Categories { get; set; }
        [BsonElement("Tabs")]
        public List<ObjectId> Tabs { get; set; }
        [BsonElement("Notes")]
        public List<ObjectId> Notes { get; set; }
    }
}