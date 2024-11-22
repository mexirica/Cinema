namespace Cinema.API.Booking.Helpers;

public class SeatHelper
{
    /// <summary>
    ///     Retrieves a RoomSeat asynchronously based on the provided seat ID.
    /// </summary>
    /// <param name="db">The database context to use for the query.</param>
    /// <param name="seatId">The ID of the seat to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the RoomSeat object.</returns>
    /// <exception cref="NotFoundException">Thrown when the seat with the specified ID is not found.</exception>
    public static async Task<RoomSeat> GetRoomSeatAsync(CinemaDbContext db, int seatId,
        CancellationToken cancellationToken = default)
    {
        var seat = await db.RoomSeats.Include(s => s.Seat)
            .FirstOrDefaultAsync(rs => rs.Seat.Id == seatId, cancellationToken);
        if (seat == null)
            throw new NotFoundException("Seat not found");

        return seat;
    }

    /// <summary>
    ///     Checks if a specific seat is reserved for a given screening.
    /// </summary>
    /// <param name="db">The database context to use for the query.</param>
    /// <param name="screeningId">The ID of the screening to check.</param>
    /// <param name="seatId">The ID of the seat to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean indicating whether the
    ///     seat is reserved.
    /// </returns>
    public static async Task<bool> IsSeatReservedAsync(CinemaDbContext db, int screeningId, int seatId,
        CancellationToken cancellationToken = default)
    {
        var bookedSeat = await db.SaleScreeningSeats
            .FirstOrDefaultAsync(ss => ss.SaleScreening.ScreeningId == screeningId && ss.SeatId == seatId,
                cancellationToken);
        return bookedSeat != null;
    }
}