using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Cinema.API.Data.Repositories;

public class CachedScreeningRepository(IScreeningRepository repository, IDistributedCache cache) : IScreeningRepository
{
    public async Task<Screening> GetByID(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"Screening_{id}";
        var cachedScreening = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedScreening != null)
        {
            return JsonSerializer.Deserialize<Screening>(cachedScreening);
        }

        var screening = await repository.GetByID(id, cancellationToken);
        if (screening != null)
        {
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(screening), cancellationToken);
        }

        return screening;
    }

    public async Task<IEnumerable<Screening>> GetAll(CancellationToken cancellationToken = default)
    {
        var cacheKey = "AllScreenings";
        var cachedScreenings = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedScreenings != null)
        {
            return JsonSerializer.Deserialize<IEnumerable<Screening>>(cachedScreenings);
        }

        var screenings = await repository.GetAll(cancellationToken);
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(screenings), cancellationToken);

        return screenings;
    }

    public async Task<Screening> Add(Screening entity, CancellationToken cancellationToken = default)
    {
        var screening = await repository.Add(entity, cancellationToken);
        await cache.RemoveAsync("AllScreenings", cancellationToken);
        return screening;
    }

    public async Task<Screening> Update(Screening entity, CancellationToken cancellationToken = default)
    {
        var screening = await repository.Update(entity, cancellationToken);
        var cacheKey = $"Screening_{entity.Id}";
        await cache.RemoveAsync(cacheKey, cancellationToken);
        await cache.RemoveAsync("AllScreenings", cancellationToken);
        return screening;
    }

    public async Task<Screening> Delete(int id, CancellationToken cancellationToken = default)
    {
        var screening = await repository.Delete(id, cancellationToken);
        var cacheKey = $"Screening_{id}";
        await cache.RemoveAsync(cacheKey, cancellationToken);
        await cache.RemoveAsync("AllScreenings", cancellationToken);
        return screening;
    }

    public async Task<(bool Success, string Message, List<SaleScreening> SaleScreenings, List<SaleScreeningSeat> SaleScreeningSeats)> 
        BookScreeningAsync(CinemaDbContext db, int screeningId, IEnumerable<int> seatsId, int saleId, CancellationToken cancellationToken)
    {
        var cacheKey = $"ReservedSeats_{screeningId}";
        var seatsIdArray = seatsId.ToArray();

        // Remover cache da lista completa de assentos do screening ao reservar
        await cache.RemoveAsync(cacheKey, cancellationToken);

        // Executar lógica original
        return await repository.BookScreeningAsync(db, screeningId, seatsId, saleId, cancellationToken);
    }

}