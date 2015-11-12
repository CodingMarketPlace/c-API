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
    public class MailsController
    {

        private string Connection = Globals.ConnectionString;

        public string createMail(string content, string id)
        {
            string query = "INSERT INTO mails (Id, id_user, content) VALUES (NULL, " + id + ", '" + content + "')";

            MySqlHelper.ExecuteNonQuery(Connection, query);
            return "ok";
        }
    }
}
