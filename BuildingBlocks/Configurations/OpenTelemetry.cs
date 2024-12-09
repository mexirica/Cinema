using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Configurations
{
	public static class OpenTelemetry
	{
		public static IServiceCollection AddOpenTelemetryMetricsAndTracing(this IServiceCollection services, string appName)
		{
			services.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService(appName))
			.WithMetrics(metrics =>
			{
				metrics.
				AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddOtlpExporter();
				
			})
			.WithTracing(tracing =>
			{
				tracing.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddEntityFrameworkCoreInstrumentation()
				.AddOtlpExporter();
			});

				return services;
		}

		public static ILoggingBuilder AddOpenTelemetryLogging(this ILoggingBuilder builder)
		{
			builder.AddOpenTelemetry(logging => logging.AddOtlpExporter());

			return builder;
		}
	}
}
