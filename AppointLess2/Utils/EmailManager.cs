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
        public static void SendConfirmationMail(string address, Guid guid)
        {
            try
            {
                //MailMessage mail = new MailMessage("you@yourcompany.com", "user@hotmail.com");
                string format = GetBodyTemplate(); 

                //MailMessage mail = new MailMessage("alr2d4@hotmail.com", address);
                MailMessage mail = new MailMessage("lahdeviitekonsultti@gmail.com", address);
                
                 //SmtpClient client = new SmtpClient()
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("lahdeviitekonsultti@gmail.com", "2wAZXSq122"),
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