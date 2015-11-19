using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingMarketPlace.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public float Budget { get; set; }
        public string IdUser { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public bool over { get; set; }
    }
}