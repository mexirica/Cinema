namespace Cinema.API.Configurations;

public static class DbExtensions
{
    /// <summary>
    ///     Adds the database context to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="connName">The connection string name.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration,
        string connName)
    {
        var connString = configuration.GetConnectionString(connName);
        services.AddDbContext<CinemaDbContext>(options => { options.UseNpgsql(connString); });
        return services;
    }

    /// <summary>
    ///     Migrates the database.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void MigrateDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
            dbContext.Database.Migrate();
        }
    }

    public static async Task Seed(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}