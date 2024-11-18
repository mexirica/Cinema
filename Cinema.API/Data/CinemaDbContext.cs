namespace Cinema.API.Data
{
	using API.Models;

	public class CinemaDbContext : DbContext
	{
		public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }

		public DbSet<Seat> Seats { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<RoomSeat> RoomSeats { get; set; }
		public DbSet<Movie> Movies { get; set; }
		public DbSet<Screening> Screenings { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Sale> Sales { get; set; }
		public DbSet<SaleScreening> SaleScreenings { get; set; }
		public DbSet<SaleScreeningSeat> SaleScreeningSeats { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SaleScreeningSeat>()
					.HasKey(sss => new { sss.SaleScreeningId, sss.SeatId });

			modelBuilder.Entity<Customer>()
					.HasIndex(c => c.Email)
					.IsUnique();

			modelBuilder.Entity<SaleScreeningSeat>()
					.HasOne(sss => sss.SaleScreening)
					.WithMany()
					.HasForeignKey(sss => sss.SaleScreeningId);
		}

	}
}
