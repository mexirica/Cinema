using BuildingBlocks.Configurations;
using BuildingBlocks.MessageBus;
using NotificationService.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISender, EmailSender>();
builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

#region Logging

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.csv",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss}, {Level}, {Message}{NewLine}{Exception}"
        , rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Services.AddSerilog();

#endregion

#region OpenTelemetry

builder.Services.AddOpenTelemetryMetricsAndTracing(builder.Environment.ApplicationName);
builder.Logging.AddOpenTelemetryLogging();

#endregion

var app = builder.Build();

app.Run();