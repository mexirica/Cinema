using Cinema.API.Configurations;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Carter;

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

#endregion

#region Data Services

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddHealthChecks()
			.AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!);
builder.Services.AddDbContext<CinemaDbContext>();

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();
app.UseExceptionHandler(opts => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
