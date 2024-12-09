using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BuildingBlocks.Configurations;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSerilogWithOpenTelemetry(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/log.csv",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss}, {Level}, {Message}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .WriteTo.OpenTelemetry()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        return builder;
    }
    
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/log.csv",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss}, {Level}, {Message}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        return builder;
    }
}
