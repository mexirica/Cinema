using System.Net;
using System.Net.Mail;

namespace NotificationService.Models
{
	public class Email : ISender
	{
		private SmtpClient _smtpClient;
		private EmailConfig _config = new();

		public Email(IConfiguration config)
		{
			config.GetSection("EmailConfig").Bind(_config);
			_smtpClient = new SmtpClient(_config.SmtpServer)
			{
				Port = _config.Port,
				Credentials = new NetworkCredential(_config.User, _config.Password),
				EnableSsl = _config.EnableSsl,
			};
		}

		public async Task SendAsync(string to, string subject, string body)
		{
			var mailMessage = new MailMessage
			{
				From = new MailAddress(_config.Address),
				Subject = subject,
				Body = body,
				IsBodyHtml = _config.HtmlBody,
			};

			mailMessage.To.Add(to);

			await _smtpClient.SendMailAsync(mailMessage);
		}
	}
}
