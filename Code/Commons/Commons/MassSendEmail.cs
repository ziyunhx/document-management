namespace Commons
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class MassSendEmail
    {
        public static void MassSend(string userEmail, string emailpassword, string userName, List<string> ToEmail, string subject, string boby)
        {
            string host = string.Empty;
            string str2 = userEmail.Split(new char[] { '@' })[userEmail.Split(new char[] { '@' }).Length - 1].ToString();
            if ((str2.Contains("126") || str2.Contains("163")) || str2.Contains("yeah"))
            {
                host = "smtp." + str2;
            }
            else
            {
                host = "mail." + str2;
            }
            SmtpClient client = new SmtpClient(host) {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(userEmail, emailpassword)
            };
            MailMessage message = new MailMessage {
                Priority = MailPriority.High,
                From = new MailAddress(userEmail, userName, Encoding.GetEncoding("UTF-8")),
                ReplyTo = new MailAddress(userEmail, "我的接收邮箱", Encoding.GetEncoding("UTF-8"))
            };
            foreach (string str3 in ToEmail)
            {
                message.Bcc.Add(new MailAddress(str3, "", Encoding.GetEncoding("UTF-8")));
            }
            message.Subject = subject;
            message.SubjectEncoding = Encoding.GetEncoding("UTF-8");
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.GetEncoding("UTF-8");
            message.Body = boby;
            try
            {
                client.Send(message);
            }
            catch
            {
                throw;
            }
        }
    }
}

