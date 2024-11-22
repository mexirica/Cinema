namespace NotificationService.Models;

public class EmailConfig
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
    public string Address { get; set; }
    public bool HtmlBody { get; set; }
}