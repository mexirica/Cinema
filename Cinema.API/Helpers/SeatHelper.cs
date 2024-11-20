using Cinema.API.Models;

namespace Cinema.API.Helpers
{
    public static class SeatHelper
    {
        public static async Task<RoomSeat> GetRoomSeatAsync(CinemaDbContext db, int seatId, CancellationToken cancellationToken)
        {
            var seat = await db.RoomSeats.FirstOrDefaultAsync(rs => rs.Seat.Id == seatId, cancellationToken);
            if (seat == null)
                throw new NotFoundException("Seat not found");

            return seat;
        }

        public static async Task<bool> IsSeatReservedAsync(CinemaDbContext db, int screeningId, int seatId, CancellationToken cancellationToken)
        {
            var bookedSeat = await db.SaleScreeningSeats
                .FirstOrDefaultAsync(ss => ss.SaleScreening.ScreeningId == screeningId && ss.SeatId == seatId, cancellationToken);
            return bookedSeat != null;
        }
    }
}