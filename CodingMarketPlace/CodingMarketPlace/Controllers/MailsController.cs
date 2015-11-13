using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;

namespace CodingMarketPlace.Controllers
{
    public class MailsController
    {

        private string Connection = Globals.ConnectionString;

        public string createMail(string content, int id, string contactMailAddress)
        {
            string emailAddress = "codingmarketplace@gmail.com", password = "y5314a!h5Xn94";

            var sender = new GmailDotComMail(emailAddress, password);
            sender.SendMail(contactMailAddress, "Coding MarketPlace - contact", content);

            string query = "INSERT INTO mails (Id, id_user, content) VALUES (NULL, " + id + ", '" + content + "')";
            MySqlHelper.ExecuteNonQuery(Connection, query);
            return "ok";
        }

        static bool RedirectionCallback(string url)
        {
            // Return true if the URL is an HTTPS URL.
            return url.ToLower().StartsWith("https://");
        }
    }
}
