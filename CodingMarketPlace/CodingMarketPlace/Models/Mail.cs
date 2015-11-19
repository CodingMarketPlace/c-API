using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingMarketPlace.Models
{
    public class Mail
    {
        public int Id { get; set; }
        public string IdUser { get; set; }
        public string Content { get; set; }
    }
}