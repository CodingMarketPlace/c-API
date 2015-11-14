using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
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
                    using (MySqlDataReader mailChecker = MySqlHelper.ExecuteReader(Connection, "SELECT Email From users WHERE id = '" + mail.IdUser + "'"))
                    {
                        if (mailChecker.HasRows)
                        {
                            mailChecker.Read();
                            MailsController mailC = new MailsController();
                            if (mailC.createMail(mail.Content, mail.IdUser, mailChecker.GetString(0)).Equals("ok"))
                            {
                                return Request.CreateResponse(HttpStatusCode.Created, "Mail envoyé avec succès");
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Erreur lors de la création du mail");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Mauvais destinataire");
                        }
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Vous n'êtes pas autorisé à envoyer un mail");
                }
            }
        }

    }
}
