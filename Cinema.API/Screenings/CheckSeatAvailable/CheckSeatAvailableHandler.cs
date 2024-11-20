using Cinema.API.Helpers;

namespace Cinema.API.Screenings.CheckSeatAvailable;

public record CheckSeatAvailableQuery(int ScreeningId, int SeatId) : IQuery<CheckSeatAvailableResponse>;

public class CheckSeatAvailableHandler(CinemaDbContext db) : IQueryHandler<CheckSeatAvailableQuery,CheckSeatAvailableResponse>
{
    public async Task<CheckSeatAvailableResponse> Handle(CheckSeatAvailableQuery request, CancellationToken cancellationToken)
    {
        var screening = await ScreeningHelper.GetScreeningAsync(db, request.ScreeningId, cancellationToken);

        var seat = await SeatHelper.GetRoomSeatAsync(db, request.SeatId, cancellationToken);

        var isSeatReserved = await SeatHelper.IsSeatReservedAsync(db, request.ScreeningId, request.SeatId, cancellationToken);

        return new CheckSeatAvailableResponse(!isSeatReserved, isSeatReserved ? "Seat already taken" : "Seat available");
    }
}