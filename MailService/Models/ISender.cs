namespace NotificationService.Models
{
	public interface ISender
	{
		Task SendAsync(string to, string subject, string body);
	}
}
