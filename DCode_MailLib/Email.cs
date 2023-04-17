using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace DCode_MailLib
{
    public class Email
    {
        public string Provider { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public short Port { get; set; } = 587;
        public int TimeOut { get; set; } = 50000;

        public Email(string provider, string username, string password, short port = 587, int timeout = 50000)
        {
            this.Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.UserName = username ?? throw new ArgumentNullException(nameof(username));
            this.PassWord = password ?? throw new ArgumentNullException(nameof(password));
            this.Port = port;
            this.TimeOut = timeout;
        }        

        public void SendEmail(List<string> emailsTo, string subject, string body, List<string>? attachments = null)
        {
            var msg = PreparateMsg(emailsTo, subject, body, attachments);

            SendEmailBySmtp(msg);
        }
       

        private MailMessage PreparateMsg(List<string> emailsTo, string subject, string body, List<string>? attachments)
        {
            var mail = new MailMessage();
            mail.From =new MailAddress(UserName);

            foreach (var email in emailsTo)
                mail.To.Add(email);

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            if(attachments!=null)
            {
                foreach (var file in attachments)
                {
                    var data = new Attachment(file, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition ?? throw new ArgumentException("PreparateMsg: não foi possível identificar o [disposition!]");
                    disposition.CreationDate = File.GetCreationTime(file);
                    disposition.ModificationDate = File.GetLastWriteTime(file);
                    disposition.ReadDate = File.GetLastAccessTime(file);

                    mail.Attachments.Add(data);
                }
            }
           

            return mail;
        }

        private void SendEmailBySmtp(MailMessage message)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = this.Provider;
            smtpClient.Port = this.Port;
            smtpClient.EnableSsl = true;
            smtpClient.Timeout = this.TimeOut;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(this.UserName, this.PassWord);
            smtpClient.Send(message);
            smtpClient.Dispose();
        }
    }
}
