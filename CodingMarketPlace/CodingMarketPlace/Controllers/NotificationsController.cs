using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace CodingMarketPlace.Controllers
{
    public class NotificationsController : ApiController
    {

        private string Connection = Globals.ConnectionString;

        /// <summary>
        /// Get all notifications for a user
        /// </summary>
        /// <param name="id">user id</param>
        /// <remarks>Get all notifications for a user</remarks>
        /// <response code="200">List successfully returned</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ActionName("AllForUser")]
        public object getAllNotificationsForUser(string id)
        {
            List<Notification> notifications = new List<Notification>();

            string query = "SELECT id_user, texte, already_read From notifications where id_user = '" + id + "'";

            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, query))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Notification notification = new Notification();
                        notification.UniqId = reader.GetString(0);
                        notification.Text = reader.GetString(1);
                        notification.Read = reader.GetBoolean(2);
                        notifications.Add(notification);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, notifications);
                }
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal server error");
        }

        /// <summary>
        /// Set all notifications to read for a user
        /// </summary>
        /// <param name="id">user id</param>
        /// <remarks>Set all notifications to read for a user</remarks>
        /// <response code="200">List successfully updated</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ActionName("ReadForUser")]
        public object setAllNotificationsReadForUser(string id)
        {
            List<Notification> notifications = new List<Notification>();

            string query = "UPDATE notifications SET already_read = 1 WHERE id_user = '" + id + "'";

            MySqlHelper.ExecuteNonQuery(Connection, query);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void createNotification(Notification notif)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id From users WHERE uniq_id = '" + notif.UniqId + "'"))
            {
                if (userChecker.HasRows)
                {
                    userChecker.Read();
                    if (userChecker.GetBoolean(0))
                    {
                        string query = "INSERT INTO notifications (Id, id_user, texte, already_read) VALUES (NULL, @userId, @text, 0)";

                        DateTime localDate = DateTime.Now;

                        // Create the parameters
                        List<MySqlParameter> parms = new List<MySqlParameter>();
                        parms.Add(new MySqlParameter("userId", notif.UniqId));
                        parms.Add(new MySqlParameter("text", notif.Text));

                        MySqlHelper.ExecuteNonQuery(Connection, query, parms.ToArray());
                    }
                }
            }
        }
    }
}
