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

        //Méthodes POST

        [HttpPost]
        [ActionName("Send")]
        public object Send([FromBody] Mail mail, string id)
        {
            MailsController mailC = new MailsController();

            if (mailC.createMail(mail.Content, id).Equals("ok"))
            {
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }
        }

    }
}
