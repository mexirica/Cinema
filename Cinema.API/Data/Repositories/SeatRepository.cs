namespace Cinema.API.Data.Repositories;

public class SeatRepository(CinemaDbContext db) : ISeatRepository
{
    public async Task<Seat> GetByID(int id, CancellationToken cancellationToken = default)
    {
        var seat = await db.Seats.FindAsync(new object[] { id }, cancellationToken);
        if (seat == null)
            throw new NotFoundException("Seat not found");

        return seat;
    }

    public async Task<IEnumerable<Seat>> GetAll(CancellationToken cancellationToken = default)
    {
        return await db.Seats.ToListAsync(cancellationToken);
    }

    public async Task<Seat> Add(Seat entity, CancellationToken cancellationToken = default)
    {
        await db.Seats.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Seat> Update(Seat entity, CancellationToken cancellationToken = default)
    {
        db.Seats.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Seat> Delete(int id, CancellationToken cancellationToken = default)
    {
        var seat = await GetByID(id, cancellationToken);
        if (seat != null)
        {
            db.Seats.Remove(seat);
            await db.SaveChangesAsync(cancellationToken);
        }
        return seat;
    }

    public async Task<RoomSeat> GetRoomSeatAsync(int seatId, CancellationToken cancellationToken = default)
    {
        var seat = await db.RoomSeats.Include(s => s.Seat)
            .FirstOrDefaultAsync(rs => rs.Seat.Id == seatId, cancellationToken);
        if (seat == null)
            throw new NotFoundException("Seat not found");

        return seat;
    }

    public async Task<bool> IsSeatReservedAsync(int screeningId, int seatId, CancellationToken cancellationToken = default)
    {
        var bookedSeat = await db.SaleScreeningSeats
            .FirstOrDefaultAsync(ss => ss.SaleScreening.ScreeningId == screeningId && ss.SeatId == seatId, cancellationToken);
        return bookedSeat != null;
    }
}