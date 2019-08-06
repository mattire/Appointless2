using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.IO;
using System.Configuration;

namespace AppointLess2.Utils
{
    public class EmailManager
    {
        //private static string Sender = "lahdeviitekonsultti@gmail.com";
        private static string Sender = ConfigurationManager.AppSettings["SupportEmailAddr"];

        public static void SendConfirmationMail(string address, Guid guid)
        {
            try
            {
                //MailMessage mail = new MailMessage("you@yourcompany.com", "user@hotmail.com");
                string format = GetBodyTemplate(); 

                //MailMessage mail = new MailMessage("alr2d4@hotmail.com", address);
                MailMessage mail = new MailMessage(Sender, address);
                
                 //SmtpClient client = new SmtpClient()
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(Sender, "2wAZXSq122"),
                    EnableSsl = true,
                };

                var host = ConfigurationManager.AppSettings["HostAddressName"];
                

                // cut out http:// or https:// to fool gmail warnings
                if ((host.StartsWith("http://"))) { host = host.Substring(7); }
                if ((host.StartsWith("https://"))) { host = host.Substring(8); }

                mail.Subject = "TBooking ajanvarauksen vahvistaminen";
                //mail.Body = string.Format("Clikkaa tätä linkkiä tapauksen vahvistamiseksi: http://localhost:3998/Confirmation/Confirm?strGuid={0}",guid);
                mail.Body = string.Format(format, host, guid);
                client.Send(mail);
                Console.WriteLine("Sent");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.WriteLine(exp.ToString());
                throw;
            }
        }

        public static bool SendPassRecovery(string email, string callbackUrl) {
            var client = GetClient();
            var host = ConfigurationManager.AppSettings["HostAddressName"];
            //if ((host.StartsWith("http://"))) { host = host.Substring(7); }
            //if ((host.StartsWith("https://"))) { host = host.Substring(8); }
            MailMessage mail = new MailMessage(Sender, email);
            mail.Subject = "Reset Password";
            mail.Body = "Kopioi seuraava osoite selaimen osoiteriville salasanan uudelleen asettamista varten:" + callbackUrl;
            //mail.Body = "Kopioi seuraava osoite selaimen osoiteriville salasanan uudelleen asettamista varten <a href=\"" + callbackUrl + "\">here</a>";
            client.Send(mail);
            Console.WriteLine("Sent");
            return true;
        }

        private static SmtpClient GetClient()
        {
            return new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(Sender, "2wAZXSq122"),
                EnableSsl = true,
            };
        }

        public static string GetBodyTemplate() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AppointLess2.Resources.EmailTemplate.txt";
            
            string result;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}