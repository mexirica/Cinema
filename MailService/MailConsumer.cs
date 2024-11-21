using BuildingBlocks.MessageBus;
using MassTransit;
using NotificationService.Models;

namespace MailService;

public class MailConsumer(ISender _sender) : IConsumer<Message>
{
	public async Task Consume(ConsumeContext<Message> context)
	{
		var message = context.Message;

		await _sender.SendAsync(message.To, message.Subject, message.Body);
	}
}
