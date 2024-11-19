namespace Cinema.API.Configurations
{
	public static class DbExtensions
	{
		public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration,string connName)
		{
			Console.WriteLine($"Connection String: {configuration["ConnectionStrings:Database"]}");
			var connString = configuration.GetConnectionString(connName);
			services.AddDbContext<CinemaDbContext>(options =>
			{
				options.UseNpgsql(connString);
			});
			return services;
		}
		
		public static void MigrateDatabase(this WebApplication app)
		{
			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
				dbContext.Database.Migrate();
			}
		}
	}
}
