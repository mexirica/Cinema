using BuildingBlocks.Behaviors;
using BuildingBlocks.Configurations;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.MessageBus;
using Cinema.API.Configurations;
using Cinema.API.Data.Repositories;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Application Services

var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddMessageBroker(builder.Configuration);

#endregion

#region Data Services

var conn = builder.Configuration["ChoosedDatabase"]
           ?? throw new ArgumentException("Choosed database not found");

builder.Services.AddDatabase(builder.Configuration, conn);

builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.Configuration = builder.Configuration.GetConnectionString("Redis")!;
});

builder.Services.AddScoped<IScreeningRepository, ScreeningRepository>();
builder.Services.Decorate<IScreeningRepository, CachedScreeningRepository>();


builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.Decorate<ISeatRepository, CachedSeatRepository>();

builder.Services.AddDbContext<CinemaDbContext>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString(conn)!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

#endregion

#region Logging

builder.AddSerilogWithOpenTelemetry();

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

#region OpenTelemetry

builder.Services.AddOpenTelemetryMetricsAndTracing(builder.Environment.ApplicationName);
builder.Logging.AddOpenTelemetryLogging();
#endregion

var app = builder.Build();
app.MapCarter();

app.MigrateDatabase();

app.UseExceptionHandler(opts => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();

app.Run();

Log.CloseAndFlush();