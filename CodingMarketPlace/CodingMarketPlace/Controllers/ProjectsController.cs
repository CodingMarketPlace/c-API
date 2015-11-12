using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodingMarketPlace.Controllers
{
    public class ProjectsController : ApiController
    {

        private string Connection = Globals.ConnectionString;

        //Méthodes POST

        [HttpPost]
        [ActionName("Create")]
        public object Create([FromBody] Project project)
        {
            string query = "INSERT INTO projects (Id, title, description, duration, budget, id_user, image_url, creation_date) VALUES (NULL, @title, @description, @duration, @budget, @id_user, @image_url, @creation_date)";
            
            DateTime localDate = DateTime.Now;

            // Create the parameters
            List<MySqlParameter> parms = new List<MySqlParameter>();
            parms.Add(new MySqlParameter("title", project.Title));
            parms.Add(new MySqlParameter("description", project.Description));
            parms.Add(new MySqlParameter("duration", project.Duration));
            parms.Add(new MySqlParameter("budget", project.Budget));
            parms.Add(new MySqlParameter("id_user", project.IdUser));
            parms.Add(new MySqlParameter("image_url", project.ImageUrl));
            parms.Add(new MySqlParameter("creation_date", localDate));

            MySqlHelper.ExecuteNonQuery(Connection, query, parms.ToArray());

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [HttpPost]
        [ActionName("Update")]
        public object Update([FromBody] Project project, string id)
        {
            if (id != "")
            {
                string query = "UPDATE projects SET ";

                if (project.Title != "")
                {
                    query += "Title = '" + project.Title + "'";
                    if (project.Description != "")
                    {
                        query += ", ";
                    }
                }
                if (project.Description != "")
                {
                    query += "Description = '" + project.Description + "'";
                    if (project.Duration != 0)
                    {
                        query += ", ";
                    }
                }
                if (project.Duration != 0)
                {
                    query += "Duration = " + project.Duration;
                    if (project.Budget != 0)
                    {
                        query += ", ";
                    }
                }
                if (project.Budget != 0)
                {
                    query += "Budget = " + project.Budget;
                    if (project.IdUser != 0)
                    {
                        query += ", ";
                    }
                }
                if (project.IdUser != 0)
                {
                    query += "id_user = " + project.IdUser;
                    if (project.ImageUrl != "")
                    {
                        query += ", ";
                    }
                }
                if (project.ImageUrl != "")
                {
                    query += "image_url = '" + project.ImageUrl + "'";
                }

                query += " WHERE id = '" + id + "'";

                MySqlHelper.ExecuteNonQuery(Connection, query);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
        }

        [HttpPost]
        [ActionName("Apply")]
        public object ApplyToProject([FromBody] Project project, string id)
        {
            InscriptionsController insc = new InscriptionsController();

            if (insc.createInscription(project, id).Equals("ok"))
            {
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }
        }

        [HttpPost]
        [ActionName("Validate")]
        public object Validate([FromBody] Project project, string id)
        {
            InscriptionsController insc = new InscriptionsController();

            if (insc.validateInscription(project, id).Equals("ok"))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }
        }

        //Méthodes GET

        [HttpGet]
        [ActionName("All")]
        public IEnumerable<Project> getAllProjects()
        {
            List<Project> projects = new List<Project>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT title, description, duration, budget, id_user, image_url, creation_date From projects"))
            {
                // Check if the reader returned any rows
                if (reader.HasRows)
                {
                    // While the reader has rows we loop through them,
                    // create new users, and insert them into our list
                    while (reader.Read())
                    {
                        Project project = new Project();
                        project.Title = reader.GetString(0);
                        project.Description = reader.GetString(1);
                        project.Duration = reader.GetInt32(2);
                        project.Budget = reader.GetInt32(3);
                        project.IdUser = reader.GetInt32(4);
                        project.ImageUrl = reader.GetString(5);
                        project.CreationDate = reader.GetDateTime(6);
                        projects.Add(project);
                    }
                }
            }
            return projects;
        }

        [HttpGet]
        [ActionName("Detail")]
        public object GetProjectDetail(string id)
        {
            Project response = new Project();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT title, description, duration, budget, id_user, image_url, creation_date From projects WHERE id = " + id))
            {
                // Check if the reader returned any rows
                if (reader.HasRows)
                {
                    // While the reader has rows we loop through them,
                    // create new users, and insert them into our list
                    reader.Read();
                    response.Title = reader.GetString(0);
                    response.Description = reader.GetString(1);
                    response.Duration = reader.GetInt32(2);
                    response.Budget = reader.GetInt32(3);
                    response.IdUser = reader.GetInt32(4);
                    response.ImageUrl = reader.GetString(5);
                    response.CreationDate = reader.GetDateTime(6);
                    return response;
                }
            }
            response.Id = -1;
            return response;
        }

        //Méthodes DELETE

        [HttpDelete]
        [ActionName("Delete")]
        public object Delete(string id)
        {
            string query = "DELETE FROM projects where id = " + id;

            MySqlHelper.ExecuteNonQuery(Connection, query);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
