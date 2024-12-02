using Cinema.API.Data.Repositories;
using Cinema.API.Exceptions;

namespace Cinema.API.Data;


    public class ScreeningRepository(CinemaDbContext context, SeatRepository seatRepository) : IScreeningRepository
    {
        public async Task<Screening> GetByID(int id, CancellationToken cancellationToken = default)
        {
            var screening = await context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ThenInclude(room => room.RoomSeat)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (screening == null)
                throw new NotFoundException("Screening not found");

            if (screening.Date < DateTime.UtcNow)
                throw new ScreeningAlreadyPassedException();

            return screening;
        }

        public async Task<IEnumerable<Screening>> GetAll(CancellationToken cancellationToken = default)
        {
            return await context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ThenInclude(room => room.RoomSeat)
                .ToListAsync(cancellationToken);
        }

        public async Task<Screening> Add(Screening entity, CancellationToken cancellationToken = default)
        {
            await context.Screenings.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Screening> Update(Screening entity, CancellationToken cancellationToken = default)
        {
            context.Screenings.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<Screening> Delete(int id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByID(id, cancellationToken);
            if (entity == null) return entity;
            context.Screenings.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<(bool Success, string Message, List<SaleScreening> SaleScreenings, List<SaleScreeningSeat>
            SaleScreeningSeats)>
        BookScreeningAsync(CinemaDbContext db, int screeningId, IEnumerable<int> seatsId, int saleId,
            CancellationToken cancellationToken)
    {
        var seatsIdArray = seatsId.ToArray();

        if (seatsIdArray.Length == 0) throw new BadRequestException("No seats selected");

        var screening = await GetByID(screeningId, cancellationToken);
        if (screening.IsAlreadyPassed())
            throw new ScreeningAlreadyPassedException(
                $"Screening for the movie {screening.Movie.Title}, date {screening.Date}, already passed");

        var seats = await db.Seats.Where(s => seatsIdArray.Contains(s.Id)).ToListAsync(cancellationToken);

        if (seats.Count != seatsIdArray.Length)
            return (false, "One or more seats not found", null, null);

        var seatsTaken = new List<int>();
        foreach (var seat in seats)
        {
            var isReserved = await seatRepository.IsSeatReservedAsync(screening.Id, seat.Id, cancellationToken);
            if (isReserved) seatsTaken.Add(seat.Id);
        }

        if (seatsTaken.Count > 0)
            return (false,
                $"Seats {string.Join(',', seatsTaken)} are already reserved for movie {screening.Movie.Title} at {screening.Time}.",
                null, null);

        var saleScreenings = new List<SaleScreening>();
        var saleScreeningSeats = new List<SaleScreeningSeat>();

        foreach (var seat in seats)
        {
            var saleScreening = new SaleScreening
            {
                ScreeningId = screeningId,
                SaleId = saleId,
                UnassignedSeat = false
            };
            saleScreenings.Add(saleScreening);

            var saleScreeningSeat = new SaleScreeningSeat
            {
                SaleScreening = saleScreening,
                SeatId = seat.Id
            };
            saleScreeningSeats.Add(saleScreeningSeat);
        }

        var msg = seats.Count == 1 ? "Seat" : "Seats";

        return (true, $"{msg} successfully booked", saleScreenings, saleScreeningSeats);
    }
    }