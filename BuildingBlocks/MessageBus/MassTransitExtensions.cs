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
		/// <summary>
		/// Configures and adds MassTransit with RabbitMQ as the message broker to the service collection. 
		/// Supports both publishing and consuming messages, optionally registering consumers from a specified assembly.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to which the MassTransit services are added.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> used to retrieve RabbitMQ settings (host, username, password).</param>
		/// <param name="assembly">
		/// An optional <see cref="Assembly"/> containing the consumer implementations to be registered. 
		/// If null, no consumers are registered.
		/// </param>
		/// <returns>
		/// The updated <see cref="IServiceCollection"/> with the configured MassTransit and RabbitMQ services.
		/// </returns>
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
