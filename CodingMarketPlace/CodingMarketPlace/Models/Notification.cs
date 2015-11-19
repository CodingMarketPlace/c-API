using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingMarketPlace.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UniqId { get; set; }
        public string Text { get; set; }
        public bool Read { get; set; }
    }
}