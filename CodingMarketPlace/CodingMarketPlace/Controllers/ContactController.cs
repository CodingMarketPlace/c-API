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

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="mail">Mail Model</param>
        /// <param name="id">sender's id</param>
        /// <remarks>Send an email after having checked that you are logged and that the recipient is one of our users</remarks>
        /// <response code="201">Mail successfully sent</response>
        /// <response code="400">Wrong recipient</response>
        /// <response code="405">You are not a user</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ActionName("Send")]
        public object Send([FromBody] Mail mail, string id)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id From users WHERE uniq_id = '" + id + "'"))
            {
                if (userChecker.HasRows)
                {
                    using (MySqlDataReader mailChecker = MySqlHelper.ExecuteReader(Connection, "SELECT Email, uniq_id From users WHERE uniq_id = '" + mail.IdUser + "'"))
                    {
                        if (mailChecker.HasRows)
                        {
                            mailChecker.Read();
                            MailsController mailC = new MailsController();
                            if (mailC.createMail(mail.Content, mailChecker.GetString(1), mailChecker.GetString(0)).Equals("ok"))
                            {
                                return Request.CreateResponse(HttpStatusCode.Created, "Mail envoyé avec succès");
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Erreur lors de la création du mail");
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
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "Vous n'êtes pas autorisé à envoyer un mail");
                }
            }
        }

    }
}
