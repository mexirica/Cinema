using System.Net;
using System.Net.Mail;

namespace NotificationService.Models
{
	/// <summary>
	/// Represents an email sender that uses SMTP to send emails.
	/// </summary>
	public class EmailSender : ISender
	{
		private SmtpClient _smtpClient;
		private EmailConfig _config = new();

		public EmailSender(IConfiguration config)
		{
			config.GetSection("EmailConfig").Bind(_config);
			_smtpClient = new SmtpClient(_config.SmtpServer)
			{
				Port = _config.Port,
				Credentials = new NetworkCredential(_config.User, _config.Password),
				EnableSsl = _config.EnableSsl,
			};
		}

		/// <summary>
		/// Sends an email asynchronously.
		/// </summary>
		/// <param name="to">The receiver email address.</param>
		/// <param name="subject">The subject of the email.</param>
		/// <param name="body">The body content of the email.</param>
		/// <returns>A task that represents the asynchronous send operation.</returns>
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
