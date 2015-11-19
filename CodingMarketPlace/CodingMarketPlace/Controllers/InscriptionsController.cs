using CodingMarketPlace.Models;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodingMarketPlace.Controllers
{
    public class InscriptionsController
    {

        private string Connection = Globals.ConnectionString;

        public string createInscription(Project project, string id)
        {
            string query = "INSERT INTO inscriptions (Id, id_user, id_project, Validated) VALUES (NULL, " + id + ", " + project.Id + ", false)";
            MySqlHelper.ExecuteNonQuery(Connection, query);

            using (MySqlDataReader projectChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id_user, title From projects WHERE id = '" + project.Id + "'"))
            {
                if (projectChecker.HasRows)
                {
                    projectChecker.Read();
                    using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT Email From users WHERE uniq_id = '" + projectChecker.GetString(0) + "'"))
                    {
                        if (userChecker.HasRows)
                        {
                            userChecker.Read();

                            string emailAddress = "codingmarketplace@gmail.com", password = "GSL5Ty5Botp0LMCB12^t";

                            var sender = new GmailDotComMail(emailAddress, password);
                            sender.SendMail(userChecker.GetString(0), "Coding MarketPlace - inscription au projet", "Un développeur s'est inscrit à votre projet : " + projectChecker.GetString(1));

                            Notification notif = new Notification();
                            NotificationsController notifCtrl = new NotificationsController();
                            notif.Text = "Un développeur s'est inscrit au projet : " + projectChecker.GetString(1);
                            notif.UniqId = projectChecker.GetString(0);
                            notifCtrl.createNotification(notif);
                        }
                    }
                }
            }

            return "ok";
        }

        public string validateInscription(Project project, string id)
        {
            string query = "UPDATE inscriptions SET Validated = true WHERE id_user = " + id + " AND id_project = " + project.Id;
            MySqlHelper.ExecuteNonQuery(Connection, query);
            deleteOtherApply(project, id);

            using (MySqlDataReader projectChecker = MySqlHelper.ExecuteReader(Connection, "SELECT id_user, title From projects WHERE id = '" + project.Id + "'"))
            {
                if (projectChecker.HasRows)
                {
                    projectChecker.Read();
                    using (MySqlDataReader userChecker = MySqlHelper.ExecuteReader(Connection, "SELECT Email From users WHERE uniq_id = '" + projectChecker.GetString(0) + "'"))
                    {
                        if (userChecker.HasRows)
                        {
                            userChecker.Read();

                            string emailAddress = "codingmarketplace@gmail.com", password = "GSL5Ty5Botp0LMCB12^t";

                            var sender = new GmailDotComMail(emailAddress, password);
                            sender.SendMail(userChecker.GetString(0), "Coding MarketPlace - inscription au projet", "Le projet : " + projectChecker.GetString(1) + " a bien été validé");

                            Notification notif = new Notification();
                            NotificationsController notifCtrl = new NotificationsController();
                            notif.Text = "Le projet : " + projectChecker.GetString(1) + "a bien été validé";
                            notif.UniqId = projectChecker.GetString(0);
                            notifCtrl.createNotification(notif);
                        }
                    }
                }
            }

            return "ok";                
            
        }

        public string deleteOtherApply(Project project, string id)
        {
            string query = "DELETE FROM inscriptions WHERE id_project = " + project.Id + " AND id_user != " + id;
            MySqlHelper.ExecuteNonQuery(Connection, query);
            return "ok"; 
        }
    }
}
