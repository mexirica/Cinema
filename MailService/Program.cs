using BuildingBlocks.MessageBus;
using MailService;
using NotificationService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISender, Email>();
builder.Services.AddMassTransitConsumer<MailConsumer>(builder.Configuration);

var app = builder.Build();


app.Run();