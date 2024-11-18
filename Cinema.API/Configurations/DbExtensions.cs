namespace Cinema.API.Configurations
{
	public static class DbExtensions
	{
		public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			var connString = configuration.GetConnectionString("Postgres");
			services.AddDbContext<CinemaDbContext>(options =>
			{
				options.UseNpgsql(connString);
			});
			return services;
		}
	}
}
