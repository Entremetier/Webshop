using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public static class MailService
    {
        public static void SendMail(string firstName, string lastName, string email, Stream pdf)
        {
            Attachment attachment = new Attachment(pdf, $"{email}Rechnung.pdf");
            var message = new MailMessage(@"tronShop@gmx.at", email)
            {
                Subject = "Rechnung Tron",
                Body = "Hallo " + firstName + " " + lastName + ", \n\n im Anhang findest Du deine Rechnung vom " + 
                        DateTime.Now.ToShortDateString() + ".\n\n Vielen Dank für deinen Einkauf bei Tron Webshop"
            };
            message.Attachments.Add(attachment);
            SmtpClient mailer = new SmtpClient("mail.gmx.net", 587); // das wären die Servereinstellungen für die qualimail mit Port
            mailer.Credentials = new NetworkCredential("tronShop@gmx.at", "Entremetier82"); // Anmeldedaten zum Emailaccount übergeben
            mailer.EnableSsl = true;
            mailer.Send(message);
        }
    }
}
