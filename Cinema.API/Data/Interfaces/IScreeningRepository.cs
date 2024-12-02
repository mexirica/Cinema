using Cinema.API.Exceptions;

namespace Cinema.API.Data;

public interface IScreeningRepository : IRepository<Screening>
{
    Task<(bool Success, string Message, List<SaleScreening> SaleScreenings, List<SaleScreeningSeat>
            SaleScreeningSeats)>
        BookScreeningAsync(CinemaDbContext db, int screeningId, IEnumerable<int> seatsId, int saleId,
            CancellationToken cancellationToken);

}