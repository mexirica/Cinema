using BuildingBlocks.MessageBus;
using NotificationService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISender, EmailSender>();
builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();


app.Run();