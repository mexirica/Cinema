using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace BuildingBlocks.MessageBus
{
	sealed class RabbitMqConfiguration
	{
		public string Host { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public static class MassTransitExtensions
	{
		public static IServiceCollection AddMessageBroker(this IServiceCollection services,
			IConfiguration configuration, Assembly? assembly = null)
		{
			services.AddMassTransit(config =>
			{
				config.SetKebabCaseEndpointNameFormatter();

				if (assembly != null)
					config.AddConsumers(assembly);

				config.UsingRabbitMq((context, configurator) =>
				{
					configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
					{
						host.Username(configuration["MessageBroker:UserName"]!);
						host.Password(configuration["MessageBroker:Password"]!);
					});
					configurator.ConfigureEndpoints(context);
				});
			});

			return services;
		}
	}
}
