namespace Cinema.API.Data;

public interface ISeatRepository: IRepository<Seat>
{
    Task<RoomSeat> GetRoomSeatAsync(int seatId,
        CancellationToken cancellationToken = default);

    Task<bool> IsSeatReservedAsync(int screeningId, int seatId,
        CancellationToken cancellationToken = default);
}