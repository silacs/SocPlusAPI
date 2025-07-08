using SocPlus.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace SocPlus.Services; 
public class MailService {
    private readonly MailCreds _creds;
    public MailService(MailCreds creds) {
        _creds = creds;
    }
    public async Task SendCode(string code, string recipient) {
        var client = new SmtpClient("smtp-mail.outlook.com", 587);
        var creds = new NetworkCredential(_creds.Username, _creds.Password);
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Credentials = creds;
        var from = new MailAddress(_creds.Username, "no-reply");
        var to = new MailAddress(recipient);
        var message = new MailMessage(from, to);
        message.IsBodyHtml = true;
        message.BodyEncoding = Encoding.UTF8;
        message.Subject = "Your SocPlus verification code";
        message.Body = $"<h2>Your code is</h2>\r\n" +
                       $"<h1>{code}</h1>\r\n" +
                       $"<h3>Expires in 1 hour</h3>";
        await client.SendMailAsync(message);
    }
}
