using System.Net;
using System.Net.Mail;

namespace EmailSender
{
    public class FullConfiguredEmail : IEmailInterface
    {
        public void Send(string to, string message)
        {
            try
            {
                var fromAddress = new MailAddress("from@gmail.com", "From Name");
                var toAddress = new MailAddress("to@example.com", "To Name");
                const string fromPassword = "fromPassword";
                const string subject = "Subject";
                var body = message;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var emailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(emailMessage);
                }
            }
            catch {}
        }
    }
}
