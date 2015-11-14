using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
        public object Create([FromBody] Project project, string id)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT project_creator, id From users WHERE uniq_id = '" + id + "'"))
            {
                if (userChecker.HasRows)
                {
                    userChecker.Read();
                    if (userChecker.GetBoolean(0))
                    {
                        string query = "INSERT INTO projects (Id, title, description, duration, budget, id_user, image_url, creation_date) VALUES (NULL, @title, @description, @duration, @budget, @id_user, @image_url, @creation_date)";

                        DateTime localDate = DateTime.Now;

                        // Create the parameters
                        List<MySqlParameter> parms = new List<MySqlParameter>();
                        parms.Add(new MySqlParameter("title", project.Title));
                        parms.Add(new MySqlParameter("description", project.Description));
                        parms.Add(new MySqlParameter("duration", project.Duration));
                        parms.Add(new MySqlParameter("budget", project.Budget));
                        parms.Add(new MySqlParameter("id_user", userChecker.GetInt32(1)));
                        parms.Add(new MySqlParameter("image_url", project.ImageUrl));
                        parms.Add(new MySqlParameter("creation_date", localDate));

                        MySqlHelper.ExecuteNonQuery(Connection, query, parms.ToArray());
                        return Request.CreateResponse(HttpStatusCode.Created, "Project successfully created");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "You are not a project creator");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error, project could not be created");
        }

        [HttpPost]
        [ActionName("Update")]
        public object Update([FromBody] Project project, string id)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id From users WHERE uniq_id = '" + id + "'"))
            {
                if (userChecker.HasRows)
                {
                    userChecker.Read();
                    if (userChecker.GetInt32(0) == project.IdUser)
                    {
                        int cptPointsToUpdate = 0;
                        if (project.Title != "")
                        {
                            cptPointsToUpdate++;
                        }
                        if (project.Description != "")
                        {
                            cptPointsToUpdate++;
                        }
                        if (project.Duration > 0)
                        {
                            cptPointsToUpdate++;
                        }
                        if (project.Budget > 0)
                        {
                            cptPointsToUpdate++;
                        }
                        if (project.ImageUrl != "")
                        {
                            cptPointsToUpdate++;
                        }
                        string query = "UPDATE projects SET ";

                        if (project.Title != "")
                        {
                            query += "Title = '" + project.Title + "'";
                            if (cptPointsToUpdate > 0)
                            {
                                query += ", ";
                                cptPointsToUpdate--;
                            }
                        }
                        if (project.Description != "")
                        {
                            query += "Description = '" + project.Description + "'";
                            if (cptPointsToUpdate > 0)
                            {
                                query += ", ";
                                cptPointsToUpdate--;
                            }
                        }
                        if (project.Duration > 0)
                        {
                            query += "Duration = " + project.Duration;
                            if (cptPointsToUpdate > 0)
                            {
                                query += ", ";
                                cptPointsToUpdate--;
                            }
                        }
                        if (project.Budget > 0)
                        {
                            query += "Budget = " + project.Budget;
                            if (cptPointsToUpdate > 0)
                            {
                                query += ", ";
                                cptPointsToUpdate--;
                            }
                        }
                        if (project.ImageUrl != "")
                        {
                            query += "image_url = '" + project.ImageUrl + "'";
                            if (cptPointsToUpdate > 0)
                            {
                                cptPointsToUpdate--;
                            }
                        }

                        query += " WHERE id = '" + project.Id + "'";

                        MySqlHelper.ExecuteNonQuery(Connection, query);

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "You are not the project owner");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error");
        }

        [HttpPost]
        [ActionName("Apply")]
        public object ApplyToProject([FromBody] Project project, string id)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT developper From users WHERE uniq_id = '" + id + "'"))
            {
                if (userChecker.HasRows)
                {
                    userChecker.Read();
                    if (userChecker.GetBoolean(0))
                    {
                        InscriptionsController insc = new InscriptionsController();
                        if (insc.createInscription(project, id).Equals("ok"))
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "Inscription to project successful");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error, inscription to project denied");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "You are not a developper");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error, could not proceed to inscription");
        }

        [HttpPost]
        [ActionName("Validate")]
        public object Validate([FromBody] Project project, string id)
        {
            using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id From users WHERE uniq_id = '" + id + "'"))
            {
                if (userChecker.HasRows)
                {
                    userChecker.Read();
                    if (userChecker.GetInt32(0) == project.IdUser)
                    {
                        InscriptionsController insc = new InscriptionsController();
                        if (insc.validateInscription(project, id).Equals("ok"))
                        {

                            return Request.CreateResponse(HttpStatusCode.OK, "Project has been validated");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Project could not be validated");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "You are not the project owner");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error, could not proceed to validation");
        }

        //Méthodes GET

        [HttpGet]
        [ActionName("All")]
        public object getAllProjects()
        {
            List<Project> projects = new List<Project>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT title, description, duration, budget, id_user, image_url, creation_date From projects"))
            {
                if (reader.HasRows)
                {
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
            return Request.CreateResponse(HttpStatusCode.OK, projects);
        }

        [HttpGet]
        [ActionName("Detail")]
        public object GetProjectDetail(string id)
        {
            Project response = new Project();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT title, description, duration, budget, id_user, image_url, creation_date From projects WHERE id = " + id))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    response.Title = reader.GetString(0);
                    response.Description = reader.GetString(1);
                    response.Duration = reader.GetInt32(2);
                    response.Budget = reader.GetInt32(3);
                    response.IdUser = reader.GetInt32(4);
                    response.ImageUrl = reader.GetString(5);
                    response.CreationDate = reader.GetDateTime(6);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "wrong id");
                }
            }
        }

        //Méthodes DELETE

        [HttpDelete]
        [ActionName("Delete")]
        public object Delete([FromBody] User user, string id)
        {
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT Admin FROM users WHERE uniq_id = '" + id + "'"))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    if (reader.GetBoolean(0) == true)
                    {
                        string query = "DELETE FROM projects where id = " + id;
                        MySqlHelper.ExecuteNonQuery(Connection, query);
                        return Request.CreateResponse(HttpStatusCode.OK, "Project deleted successfully");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "You are not an administrator");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Deletion error");
        }
    }
}
