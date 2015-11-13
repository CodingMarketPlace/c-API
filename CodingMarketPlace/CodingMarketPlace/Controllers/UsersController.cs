using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CodingMarketPlace.Controllers
{
    public class UsersController : ApiController
    {
        private const string RivestShamirAdlemanKey = "<RSAKeyValue><Modulus>o1V/G/65iGcISEuln5n8L6YdHbxAYT05KRjLDzEIO1Wkl/fDqQDT9MNI13MUJzNGEgzXb8FEziGAXB9wdM221aP4Q6o7CAJNxuctsMCKXHwD6M+IHxAQs41Jf86zv3AGiB1yqLI3BsjrwuOIr91vkyczfhtb+qe72JQbw25yOiU=</Modulus><Exponent>AQAB</Exponent><P>2BzeAwoZw6uAGW2BVGMIS60qjijEMaZ39IgjFCwr6VX+goKUNI/wZrkdDS1t+ZMRKapcGx2Y5gZsmZLCyOtxAQ==</P><Q>wXrhChqKniRNSnaf4Spxe41gdxyuphSOAMCrlZRcP26UIrBH6f/tanC49VZr+8R2Gi1765MNJ4ih0VEiQ1XlJQ==</Q><DP>DvbgwKEga5YihqA4hlldJ7BT9AgKnc2DHOGYXDs6xyt3Nh5ImOMmqFZFFraAmPmABLyRKCeCgNsNBg1Ng5AaAQ==</DP><DQ>g2dPO6t3BZymGbKjNyu6Uy1bnMoQG5/OKdixMC/IzxPs6/pJfTViK25PT+DYCfAOPg0yInaG8pirPhwaZx0JOQ==</DQ><InverseQ>T6txQ7LSrNlKE936S2KnQE3OTTCCtlW9EsoWAh2anWlmiIi0LCrv16nTlY7NU4kh78JomDDm2Ozoi9Trb4lb3Q==</InverseQ><D>JXqbm3Koo6BS0fYLx/L3X36sTTOyiS2ZhXDjPXXol+bnyRhJHSlruY0rFIcbT4BwOnmYYNQ2I9+jmt/6985xfrfv3FZIei1sYroJweaobyQBVeRwxqB5pocL15otNhjDjZ5dc2XJNHkflmmu6J+kjuvJjrNM+jGTiAMLxxt8cQE=</D></RSAKeyValue>";

        private string Connection = Globals.ConnectionString;

        RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

        //Méthodes POST

        [HttpPost]
        [ActionName("Create")]
        public object Create([FromBody] User user)
        {
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT Login, Email From users WHERE Login = '" + user.Login + "' OR Email = '" + user.Email + "'"))
            {
                if (reader.HasRows)
                {
                    if (reader.GetString(0).Equals(user.Login))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Login already exist");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Email already exist");
                    }
                }
            }

            string query = "INSERT INTO users (Id, Email, password, login, developper, project_creator, description, image_url, first_name, last_name, uniq_id) VALUES (NULL, @email, @password, @login, @developper, @projectCreator, @description, @imageUrl, @firstName, @lastName, @uniqId)";

            Random rnd = new Random();
            int number = rnd.Next(1000, 10000);
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("fr-FR");
            string uniqId = number.ToString() + localDate.ToString(culture).Replace(" ", string.Empty).Replace("/", string.Empty).Replace(":", string.Empty);
            
            List<MySqlParameter> parms = new List<MySqlParameter>();
            parms.Add(new MySqlParameter("email", user.Email));
            parms.Add(new MySqlParameter("password", encryptString(user.Password)));
            parms.Add(new MySqlParameter("login", user.Login));
            parms.Add(new MySqlParameter("developper", user.Developper));
            parms.Add(new MySqlParameter("projectCreator", user.ProjectCreator));
            parms.Add(new MySqlParameter("description", user.Description));
            parms.Add(new MySqlParameter("imageUrl", user.ImageUrl));
            parms.Add(new MySqlParameter("firstName", user.FirstName));
            parms.Add(new MySqlParameter("lastName", user.LastName));
            parms.Add(new MySqlParameter("uniqId", uniqId));

            MySqlHelper.ExecuteNonQuery(Connection, query, parms.ToArray());

            return Request.CreateResponse(HttpStatusCode.Created, "Utilisateur créé avec succes");
        }

        private string encryptString(string stringToEncrypt)
        {
            RSA.FromXmlString(RivestShamirAdlemanKey);
            byte[] tableauchiffre = RSA.Encrypt(Encoding.Unicode.GetBytes(stringToEncrypt), false);
            return Convert.ToBase64String(tableauchiffre);
        }

        private string decryptString(string stringToDecrypt)
        {
            RSA.FromXmlString(RivestShamirAdlemanKey);
            byte[] tableau = Convert.FromBase64String(stringToDecrypt);
            return Encoding.Unicode.GetString(RSA.Decrypt(tableau, false));
        }

        [HttpPost]
        [ActionName("Update")]
        public object Update([FromBody] User user, string id)
        {
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT * From users WHERE uniq_id = '" + id + "'"))
            {
                if (reader.HasRows)
                {
                    int cptPointsToUpdate = 0;
                    if (user.Password != "")
                    {
                        cptPointsToUpdate++;
                    }
                    if (user.Email != "")
                    {
                        using (MySqlDataReader mailChecker = MySqlHelper.ExecuteReader(Connection, "SELECT Email From users WHERE uniq_id = '" + id + "'"))
                        {
                            if (mailChecker.HasRows)
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Email already exist");
                            }
                        }
                        cptPointsToUpdate++;
                    }
                    if (user.Activated != false)
                    {
                        cptPointsToUpdate++;
                    }
                    if (user.FirstName != "")
                    {
                        cptPointsToUpdate++;
                    }
                    if (user.LastName != "")
                    {
                        cptPointsToUpdate++;
                    }
                    if (user.Description != "")
                    {
                        cptPointsToUpdate++;
                    }
                    if (user.ImageUrl != "")
                    {
                        cptPointsToUpdate++;
                    }
                    string query = "UPDATE users SET ";

                    if (user.Password != "")
                    {
                        query += "Password = '" + encryptString(user.Password) + "'";
                        if (cptPointsToUpdate > 0)
                        {
                            query += ", ";
                            cptPointsToUpdate--;
                        }
                    }
                    if (user.Email != "")
                    {
                        query += "Email = '" + user.Email + "'";
                        if (cptPointsToUpdate > 0)
                        {
                            query += ", ";
                            cptPointsToUpdate--;
                        }
                    }
                    if (user.Activated != false)
                    {
                        query += "Activated = " + user.Activated;
                        if (cptPointsToUpdate > 0)
                        {
                            query += ", ";
                            cptPointsToUpdate--;
                        }
                    }
                    if (user.FirstName != "")
                    {
                        query += "first_name = '" + user.FirstName + "'";
                        if (cptPointsToUpdate > 0)
                        {
                            query += ", ";
                            cptPointsToUpdate--;
                        }
                    }
                    if (user.LastName != "")
                    {
                        query += "last_name = '" + user.LastName + "'";
                        if (cptPointsToUpdate > 0)
                        {
                            query += ", ";
                            cptPointsToUpdate--;
                        }
                    }
                    if (user.Description != "")
                    {
                        query += "description = '" + user.Description + "'";
                        if (cptPointsToUpdate > 0)
                        {
                            query += ", ";
                            cptPointsToUpdate--;
                        }
                    }
                    if (user.ImageUrl != "")
                    {
                        query += "image_Url = '" + user.ImageUrl + "'";
                        if (cptPointsToUpdate > 0)
                        {
                            cptPointsToUpdate--;
                        }
                    }

                    query += " WHERE uniq_id = '" + id + "'";

                    MySqlHelper.ExecuteNonQuery(Connection, query);

                    return Request.CreateResponse(HttpStatusCode.OK, "Utilisateur mis à jour avec succès");
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Erreur, mise à jour non effectuée");
        }

        [HttpPost]
        [ActionName("ChangeRole")]
        public object ChangeRole([FromBody] User user, string id)
        {
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT Admin FROM users WHERE uniq_id = '" + id + "'"))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    
                    if (reader.GetBoolean(0) == true)
                    {
                        string query = "UPDATE users SET Admin = " + user.Admin + ", project_creator = " + user.ProjectCreator + ", developper = " + user.Developper + " WHERE uniq_id = '" + user.UniqId + "'";

                        MySqlHelper.ExecuteNonQuery(Connection, query);

                        return Request.CreateResponse(HttpStatusCode.OK, "Rôle utilisateur mis à jour avec succès");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Vous n'êtes pas administrateur");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Rôle non changé");
        }

        //Méthodes GET

        [HttpGet]
        [ActionName("All")]
        public object GetAllUsers()
        {
            List<User> users = new List<User>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT Password, Login, Email, Uniq_id, Activated, Developper, Project_creator, first_name, last_name, admin, description, image_Url From users"))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User user = new User();
                        user.Login = reader.GetString(1);
                        user.Email = reader.GetString(2);
                        user.UniqId = reader.GetString(3);
                        user.Activated = reader.GetBoolean(4);
                        user.Developper = reader.GetBoolean(5);
                        user.ProjectCreator = reader.GetBoolean(6);
                        user.FirstName = reader.GetString(7);
                        user.LastName = reader.GetString(8);
                        user.Admin = reader.GetBoolean(9);
                        user.Description = reader.GetString(10);
                        user.ImageUrl = reader.GetString(11);
                        users.Add(user);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpPost]
        [ActionName("Login")]
        public object Login([FromBody] User user)
        {
            User response = new User();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT Password, Login, Email, Uniq_id, Activated, Developper, Project_creator, first_name, last_name, admin, description, image_Url From users WHERE login = '" + user.Login + "'"))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    string pass = decryptString(reader.GetString(0));
                    if (pass.Equals(user.Password))
                    {
                        response.Login = reader.GetString(1);
                        response.Email = reader.GetString(2);
                        response.UniqId = reader.GetString(3);
                        response.Activated = reader.GetBoolean(4);
                        response.Developper = reader.GetBoolean(5);
                        response.ProjectCreator = reader.GetBoolean(6);
                        response.FirstName = reader.GetString(7);
                        response.LastName = reader.GetString(8);
                        response.Admin = reader.GetBoolean(9);
                        response.Description = reader.GetString(10);
                        response.ImageUrl = reader.GetString(11);
                        return Request.CreateResponse(HttpStatusCode.OK, response);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "login failed");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Login Error");
        }

        [HttpGet]
        [ActionName("Detail")]
        public object GetUserDetail(string id)
        {
            User response = new User();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, "SELECT Password, Login, Email, Uniq_id, Activated, Developper, Project_creator, first_name, last_name, admin, description, image_Url From users WHERE uniq_id = " + id))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    response.Login = reader.GetString(1);
                    response.Email = reader.GetString(2);
                    response.UniqId = reader.GetString(3);
                    response.Activated = reader.GetBoolean(4);
                    response.Developper = reader.GetBoolean(5);
                    response.ProjectCreator = reader.GetBoolean(6);
                    response.FirstName = reader.GetString(7);
                    response.LastName = reader.GetString(8);
                    response.Admin = reader.GetBoolean(9);
                    response.Description = reader.GetString(10);
                    response.ImageUrl = reader.GetString(11);
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
                        string query = "DELETE FROM users where uniq_id = '" + user.UniqId + "'";
                        MySqlHelper.ExecuteNonQuery(Connection, query);
                        return Request.CreateResponse(HttpStatusCode.OK, "User deleted successfully");
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
