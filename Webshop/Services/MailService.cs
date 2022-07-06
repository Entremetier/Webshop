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
            Attachment attachment = new Attachment(pdf, $"{email}Rechnung.pdf");
            var message = new MailMessage(@"tronshop@gmx.at", email);
            message.Subject = "Rechnung Tron";
            message.Body = "Hier die Rechnung du Affe!";
            message.Attachments.Add(attachment);
            SmtpClient mailer = new SmtpClient("mail.gmx.net", 587); // das wären die Servereinstellungen für die qualimail
            mailer.Credentials = new NetworkCredential("tronshop@gmx.at", "Entremetier82"); // hier müssen Ihre Anmeldedaten zum Emailaccount drinnen stehen
            mailer.EnableSsl = true;
            mailer.Send(message);
        }
    }
}
