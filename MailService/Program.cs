using BuildingBlocks.MessageBus;
using MailService;
using NotificationService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISender, Email>();
builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();


app.Run();