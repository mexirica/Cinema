using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
		private static void ConfigureRabbitMqHost(IConfiguration configuration, IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
		{
			var rabbitmqConfig = new RabbitMqConfiguration();
			configuration.GetSection("RabbitMq").Bind(rabbitmqConfig);

			if (string.IsNullOrEmpty(rabbitmqConfig.Host) || string.IsNullOrEmpty(rabbitmqConfig.Username) || string.IsNullOrEmpty(rabbitmqConfig.Password))
			{
				throw new InvalidOperationException("RabbitMQ configuration is incomplete.");
			}

			cfg.Host($"rabbitmq://{rabbitmqConfig.Host}", h =>
			{
				h.Username(rabbitmqConfig.Username);
				h.Password(rabbitmqConfig.Password);
			});
		}

		public static IServiceCollection AddMassTransitPublisher(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMassTransit(x =>
			{
				x.UsingRabbitMq((context, cfg) =>
				{
					ConfigureRabbitMqHost(configuration, context, cfg);
				});
			});

			return services;
		}

		public static IServiceCollection AddMassTransitConsumer<T>(this IServiceCollection services, IConfiguration configuration)
				where T : class, IConsumer
		{
			services.AddMassTransit(x =>
			{
				x.AddConsumer<T>();

				x.UsingRabbitMq((context, cfg) =>
				{
					ConfigureRabbitMqHost(configuration, context, cfg);

					cfg.ReceiveEndpoint(configuration["RabbitMq:QueueName"], e =>
					{
						e.ConfigureConsumer<T>(context); 
					});
				});
			});

			return services;
		}
	}
}
