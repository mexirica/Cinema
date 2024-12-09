using BuildingBlocks.Configurations;
using BuildingBlocks.MessageBus;
using NotificationService.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISender, EmailSender>();
builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

#region Logging

builder.AddSerilogWithOpenTelemetry();

#endregion

#region OpenTelemetry

builder.Services.AddOpenTelemetryMetricsAndTracing(builder.Environment.ApplicationName);
builder.Logging.AddOpenTelemetryLogging();

#endregion

var app = builder.Build();

app.Run();