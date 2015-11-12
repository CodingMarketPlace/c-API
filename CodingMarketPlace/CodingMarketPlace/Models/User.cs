using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingMarketPlace.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public bool Activated { get; set; }
        public bool Developper { get; set; }
        public bool ProjectCreator { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Admin { get; set; }
        public string UniqId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}