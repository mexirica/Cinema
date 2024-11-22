using BuildingBlocks.MessageBus;
using MassTransit;
using NotificationService.Models;

namespace MailService;

/// <summary>
///     Represents a consumer that handles incoming messages and sends them using the provided sender (Mail,SMS...).
/// </summary>
/// <param name="_sender">The sender used to send the message.</param>
public class MailConsumer(ISender _sender) : IConsumer<Message>
{
    public async Task Consume(ConsumeContext<Message> context)
    {
        var message = context.Message;

        await _sender.SendAsync(message.To, message.Subject, message.Body);
    }
}