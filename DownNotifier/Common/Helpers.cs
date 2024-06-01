using System.Net.Mail;

namespace DownNotifier.Common
{
    public class Helpers
    {
        private readonly IConfiguration _configuration;
        public Helpers(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string? MailSend(string recipients, string subject, string body, DateTime dateTime)
        {
            try
            {
                var port = int.Parse(_configuration.GetSection("Mail:Port").Value ?? "587");
                var host = _configuration.GetSection("Mail:Host").Value;
                var enableSsl = true;
                var emailfrom = _configuration.GetSection("Mail:Address").Value ?? string.Empty;
                var password = _configuration.GetSection("Mail:Password").Value;
                var mailUniqueNumber = $"{dateTime}@kadrobu.com";

                using (var smtpClient = new SmtpClient(host, port))
                {
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential(emailfrom, password);
                    smtpClient.Send(emailfrom, recipients, subject, body);
                }
                return mailUniqueNumber;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
