using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CodingMarketPlace
{
    public class GmailDotComMail
    {
        string _sender = "";
        string _password = "";
        public GmailDotComMail(string sender, string password)
        {
            _sender = sender;
            _password = password;
        }

        public void SendMail(string recipient, string subject, string message)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            NetworkCredential credentials = new NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            using (var mail = new MailMessage(_sender.Trim(), recipient.Trim())
            {
                Subject = subject,
                Body = message
            })
            {
                client.Send(mail);
            }
        }
    }
}