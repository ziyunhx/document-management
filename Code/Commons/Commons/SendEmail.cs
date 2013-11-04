namespace Commons
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class SendEmail
    {
        public static void NetSendMail(string userEmail, string emailpassword, string userName, string ToEmail, string subject, string boby)
        {
            string host = string.Empty;
            string str2 = userEmail.Split(new char[] { '@' })[userEmail.Split(new char[] { '@' }).Length - 1].ToString();
            if ((str2.Contains("qq") || str2.Contains("163")) || str2.Contains("yeah"))
            {
                host = "smtp." + str2;
            }
            else
            {
                host = "mail." + str2;
            }
            SmtpClient client = new SmtpClient(host) {
                EnableSsl = false,
                Credentials = new NetworkCredential(userEmail, emailpassword)
            };
            MailAddress from = new MailAddress(userEmail, userName, Encoding.UTF8);
            MailAddress to = new MailAddress(ToEmail);
            MailMessage message = new MailMessage(from, to) {
                Subject = subject,
                Body = boby,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            try
            {
                client.Send(message);
                message.Dispose();
            }
            catch (SmtpFailedRecipientException exception)
            {
                throw new Exception(exception.Message, exception);
            }
        }
    }
}

