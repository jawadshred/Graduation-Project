using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace turkcell_web_app.Models
{
    public class Email
    {
        public  static void send_email()
        {
            try
            {
            SmtpClient smtpClient = new SmtpClient("smtp.mailtrap.io", 2525);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential("1a50f505a134ac", "25d34c6d3c6677");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress("turkcell.graduation.project@gmail.com", "Test Email");
            mail.To.Add(new MailAddress("libyan.noblesse@gmail.com"));
            mail.CC.Add(new MailAddress("baltu.libya@gmail.com"));
            mail.Body = "New Update\n open your account";
           
                smtpClient.Send(mail);
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}