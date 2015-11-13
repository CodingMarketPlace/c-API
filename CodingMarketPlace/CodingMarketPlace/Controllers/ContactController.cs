using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
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

        private string Connection = Globals.ConnectionString;

        //Méthodes POST

        [HttpPost]
        [ActionName("Send")]
        public object Send([FromBody] Mail mail, string id)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id From users WHERE uniq_id = '" + id + "'"))
            {
                if (userChecker.HasRows)
                {
                    MailsController mailC = new MailsController();
                    if (mailC.createMail(mail.Content, id).Equals("ok"))
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, "Mail créé avec succès");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Erreur lors de la création du mail");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Mauvais utilisateur");
                }
            }
        }

    }
}
