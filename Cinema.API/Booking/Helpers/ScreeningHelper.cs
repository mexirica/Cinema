using Cinema.API.Screenings.Exceptions;

namespace Cinema.API.Booking.Helpers;

public class ScreeningHelper
{

	public static async Task<Screening> GetScreeningAsync(CinemaDbContext db, int screeningId,
			CancellationToken cancellationToken)
	{
		var screening = await db.Screenings
				.Include(s => s.Movie)
				.Include(s => s.Room)
				.ThenInclude(room => room.RoomSeat)
				.FirstOrDefaultAsync(s => s.Id == screeningId, cancellationToken);

		if (screening == null)
			throw new NotFoundException("Screening not found");

		if (screening.Date < DateTime.UtcNow)
			throw new ScreeningAlreadyPassedException();

		return screening;
	}

	public static async Task<(bool Success, string Message)> BookScreeningAsync(CinemaDbContext db, int screeningId,
			IEnumerable<int> seatsId, int customerId, int saleId, CancellationToken cancellationToken)
	{
		var seatsIdArray = seatsId.ToArray();

		if (seatsIdArray.Length == 0) throw new BadRequestException("No seats selected");

		var screening = await GetScreeningAsync(db, screeningId, cancellationToken);
		if (screening.IsAlreadyPassed())
			throw new ScreeningAlreadyPassedException(
					$"Screening for the movie {screening.Movie.Title}, date {screening.Date}, already passed");

		var seats = await db.Seats.Where(s => seatsIdArray.Contains(s.Id)).ToListAsync(cancellationToken);

		if (seats.Count != seatsIdArray.Length)
			return (false, "One or more seats not found");

		var seatsTaken = new List<int>();
		foreach (var seat in seats)
		{
			var isReserved = await SeatHelper.IsSeatReservedAsync(db, screening.Id, seat.Id, cancellationToken);
			if (isReserved) seatsTaken.Add(seat.Id);
		}

		if (seatsTaken.Count > 0)
			return (false, $"Seats {string.Join(',', seatsTaken)} are already reserved for screening {screening.Id}.");


		var seatMsg = seats.Count == 1 ? "Seat" : "Seats";

		foreach (var seat in seats)
		{
			var saleScreening = new SaleScreening
			{
				ScreeningId = screeningId,
				SaleId = saleId,
				UnassignedSeat = false
			};
			db.SaleScreenings.Add(saleScreening);
			await db.SaveChangesAsync(cancellationToken);

			var saleScreeningSeat = new SaleScreeningSeat
			{
				SaleScreening = saleScreening,
				SeatId = seat.Id
			};
			db.SaleScreeningSeats.Add(saleScreeningSeat);
			await db.SaveChangesAsync(cancellationToken);
		}

		return (true, $"{seatMsg} successfully booked");
	}
}