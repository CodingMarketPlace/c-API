using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingMarketPlace.Models
{
    public class Inscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public bool Validated { get; set; }
    }
}