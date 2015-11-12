using CodingMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodingMarketPlace.Controllers
{
    public class ContactController : ApiController
    {

        public string Connection = "server=localhost;database=codingMarketPlace;userid='MarketPlaceAdmin';password='Passw0rd';";

        //Méthodes POST

        [HttpPost]
        [ActionName("Send")]
        public object Send([FromBody] Mail mail, string id)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
