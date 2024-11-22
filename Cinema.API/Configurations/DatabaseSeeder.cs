namespace Cinema.API.Configurations;

/// <summary>
///     Class to seed the database with initial data for tests.
/// </summary>
/// <param name="context"></param>
public class DatabaseSeeder(CinemaDbContext context)
{
    /// <summary>
    ///     Check if the database is already seeded, if not, seed it.
    /// </summary>
    public async Task SeedAsync()
    {
        await SeedCustomersAsync();
        await SeedMoviesAsync();
        await SeedRoomsAsync();
        await SeedSeatsAsync();
        await SeedRoomSeatsAsync();
        await SeedScreeningsAsync();
        await SeedSalesAsync();
        await SeedSaleScreeningsAsync();
    }

    private async Task SeedCustomersAsync()
    {
        if (!await context.Customers.AnyAsync())
        {
            var customers = new List<Customer>
            {
                new() { Name = "John Doe", Email = "johndoe@example.com" },
                new() { Name = "Jane Smith", Email = "janesmith@example.com" }
            };
            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedMoviesAsync()
    {
        if (!await context.Movies.AnyAsync())
        {
            var movies = new List<Movie>
            {
                new()
                {
                    Title = "Inception",
                    EIDR = "10.5240/1234-5678-ABCD-EFGH",
                    Duration = TimeSpan.FromMinutes(148),
                    Description = "A mind-bending thriller by Christopher Nolan.",
                    Rating = 8.8f,
                    ImageUrl = "https://example.com/inception.jpg"
                },
                new()
                {
                    Title = "The Matrix",
                    EIDR = "10.5240/5678-1234-EFGH-ABCD",
                    Duration = TimeSpan.FromMinutes(136),
                    Description = "A revolutionary sci-fi classic.",
                    Rating = 8.7f,
                    ImageUrl = "https://example.com/matrix.jpg"
                }
            };
            context.Movies.AddRange(movies);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedRoomsAsync()
    {
        if (!await context.Rooms.AnyAsync())
        {
            var rooms = new List<Room>
            {
                new() { Name = "Room A" },
                new() { Name = "Room B" }
            };
            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedSeatsAsync()
    {
        if (!await context.Seats.AnyAsync())
        {
            var seats = new List<Seat>();
            for (var row = 'A'; row <= 'C'; row++)
            for (var number = 1; number <= 10; number++)
                seats.Add(new Seat { Row = row.ToString(), Number = number.ToString() });

            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedRoomSeatsAsync()
    {
        if (!await context.RoomSeats.AnyAsync())
        {
            var rooms = await context.Rooms.ToListAsync();
            var seats = await context.Seats.ToListAsync();
            var roomSeats = new List<RoomSeat>();

            foreach (var room in rooms)
            foreach (var seat in seats)
                roomSeats.Add(new RoomSeat { RoomId = room.Id, SeatId = seat.Id });

            context.RoomSeats.AddRange(roomSeats);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedScreeningsAsync()
    {
        if (!await context.Screenings.AnyAsync())
        {
            var screenings = new List<Screening>
            {
                new()
                {
                    MovieId = context.Movies.First().Id,
                    RoomId = context.Rooms.First().Id,
                    Date = DateTime.UtcNow.AddDays(1),
                    Time = TimeSpan.FromMinutes(120),
                    Price = 25.50m
                },
                new()
                {
                    MovieId = context.Movies.Skip(1).First().Id,
                    RoomId = context.Rooms.Skip(1).First().Id,
                    Date = DateTime.UtcNow.AddDays(2),
                    Time = TimeSpan.FromMinutes(90),
                    Price = 30.00m
                }
            };
            context.Screenings.AddRange(screenings);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedSalesAsync()
    {
        if (!await context.Sales.AnyAsync())
        {
            var sales = new List<Sale>
            {
                new()
                {
                    CustomerId = context.Customers.First().Id,
                    SaleDate = DateTime.UtcNow,
                    AmountPaid = 51.00m
                }
            };
            context.Sales.AddRange(sales);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedSaleScreeningsAsync()
    {
        if (!await context.SaleScreenings.AnyAsync())
        {
            var saleScreening = new SaleScreening
            {
                SaleId = context.Sales.First().Id,
                ScreeningId = context.Screenings.First().Id,
                UnassignedSeat = true
            };
            context.SaleScreenings.Add(saleScreening);
            await context.SaveChangesAsync();
        }
    }
}