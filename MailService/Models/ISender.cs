namespace NotificationService.Models;

/// <summary>
///     Defines a contract for sending notifications to the client.
/// </summary>
public interface ISender
{
    /// <summary>
    ///     Sends a notification asynchronously.
    /// </summary>
    /// <param name="to">The receiver of the notification.</param>
    /// <param name="subject">The subject of the notification.</param>
    /// <param name="body">The body content of the notification.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendAsync(string to, string subject, string body);
}