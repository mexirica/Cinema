using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Cinema.API.Data.Repositories;

public class CachedSeatRepository(ISeatRepository repository, IDistributedCache cache) : ISeatRepository
{
    public async Task<Seat> GetByID(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"Seat_{id}";
        var cachedSeat = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedSeat != null)
        {
            return JsonSerializer.Deserialize<Seat>(cachedSeat);
        }

        var seat = await repository.GetByID(id, cancellationToken);
        if (seat != null)
        {
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(seat), cancellationToken);
        }

        return seat;
    }

    public async Task<IEnumerable<Seat>> GetAll(CancellationToken cancellationToken = default)
    {
        var cacheKey = "AllSeats";
        var cachedSeats = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedSeats != null)
        {
            return JsonSerializer.Deserialize<IEnumerable<Seat>>(cachedSeats);
        }

        var seats = await repository.GetAll(cancellationToken);
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(seats), cancellationToken);

        return seats;
    }

    public async Task<Seat> Add(Seat entity, CancellationToken cancellationToken = default)
    {
        var seat = await repository.Add(entity, cancellationToken);
        await cache.RemoveAsync("AllSeats", cancellationToken);
        return seat;
    }

    public async Task<Seat> Update(Seat entity, CancellationToken cancellationToken = default)
    {
        var seat = await repository.Update(entity, cancellationToken);
        var cacheKey = $"Seat_{entity.Id}";
        await cache.RemoveAsync(cacheKey, cancellationToken);
        await cache.RemoveAsync("AllSeats", cancellationToken);
        return seat;
    }

    public async Task<Seat> Delete(int id, CancellationToken cancellationToken = default)
    {
        var seat = await repository.Delete(id, cancellationToken);
        var cacheKey = $"Seat_{id}";
        await cache.RemoveAsync(cacheKey, cancellationToken);
        await cache.RemoveAsync("AllSeats", cancellationToken);
        return seat;
    }

    public async Task<RoomSeat> GetRoomSeatAsync(int seatId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"RoomSeat_{seatId}";
        var cachedRoomSeat = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedRoomSeat != null)
        {
            return JsonSerializer.Deserialize<RoomSeat>(cachedRoomSeat);
        }

        var roomSeat = await repository.GetRoomSeatAsync(seatId, cancellationToken);
        if (roomSeat != null)
        {
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(roomSeat), cancellationToken);
        }

        return roomSeat;
    }

    public async Task<bool> IsSeatReservedAsync(int screeningId, int seatId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"ReservedSeat_{screeningId}_{seatId}";
        var cachedIsReserved = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedIsReserved != null)
        {
            return JsonSerializer.Deserialize<bool>(cachedIsReserved);
        }

        var isReserved = await repository.IsSeatReservedAsync(screeningId, seatId, cancellationToken);
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(isReserved), cancellationToken);

        return isReserved;
    }
}