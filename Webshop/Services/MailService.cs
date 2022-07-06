using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class MailService
    {
        public static void SendMail(string email, Stream pdf)
        {
            //Attachment attachment = new Attachment(pdf, $"{email}Rechnung.pdf");
            //var message = new MailMessage(@"webshop_lap@gmx.de", email);
            //message.Subject = "MasterTest";
            //message.Body = "Test TEST";
            //message.Attachments.Add(attachment);
            //SmtpClient mailer = new SmtpClient("mail.gmx.net", 587); // das wären die Servereinstellungen für die qualimail
            //mailer.Credentials = new NetworkCredential("webshop_lap@gmx.de", "Admin2019$"); // hier müssen Ihre Anmeldedaten zum Emailaccount drinnen stehen
            //mailer.EnableSsl = true;
            //mailer.Send(message);

            Attachment attachment = new Attachment(pdf, $"{email}Rechnung.pdf");
            var message = new MailMessage(@"tronShop@gmx.at", email);
            message.Subject = "Rechnung Tron";
            message.Body = "Hier die Rechnung du Affe!";
            message.Attachments.Add(attachment);
            SmtpClient mailer = new SmtpClient("mail.gmx.net", 587); // das wären die Servereinstellungen für die qualimail
            mailer.Credentials = new NetworkCredential("tronShop@gmx.at", "Entremetier82"); // hier müssen Ihre Anmeldedaten zum Emailaccount drinnen stehen
            mailer.EnableSsl = true;
            mailer.Send(message);
        }
        }
}
