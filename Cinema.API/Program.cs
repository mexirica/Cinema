using Cinema.API.Configurations;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using FluentValidation;

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

#endregion

#region Data Services

var conn = builder.Configuration["ChoosedDatabase"]
           ?? throw new ArgumentException("Choosed database not found");

builder.Services.AddDatabase(builder.Configuration, conn).AddHealthChecks()
	.AddNpgSql(builder.Configuration.GetConnectionString(conn)!);

builder.Services.AddDbContext<CinemaDbContext>();
builder.Services.AddScoped<DatabaseSeeder>();

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

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