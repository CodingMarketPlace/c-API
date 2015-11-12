﻿using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodingMarketPlace.Controllers
{
    public class InscriptionsController : ApiController
    {

        private string Connection = Globals.ConnectionString;

        public string createInscription(Project project, string id)
        {
            string query = "INSERT INTO inscriptions (Id, id_user, id_project, Validated) VALUES (NULL, " + id + ", " + project.Id + ", false)";

            MySqlHelper.ExecuteNonQuery(Connection, query);
            return "ok";
        }

        public string validateInscription(Project project, string id)
        {
            string query = "UPDATE inscriptions SET Validated = true WHERE id_user = " + id + " AND id_project = " + project.Id;

            MySqlHelper.ExecuteNonQuery(Connection, query);

            return "ok";
        }
    }
}