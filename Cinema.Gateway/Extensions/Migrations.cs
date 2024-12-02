using Microsoft.EntityFrameworkCore;

namespace Cinema.Gateway.Extensions;

public static class Migrations
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
    }
}