using Cinema.API.Helpers;
using Cinema.API.Models;

namespace Cinema.API.Screenings.MultipleBookings;

public record MultipleBookingCommand(IEnumerable<ScreeningSeatDto> ScreeningSeats, int CustomerId) : ICommand<MultipleBookingCommandResult>;

public record MultipleBookingCommandResult(bool Success, string Message, int? SaleId);

public class MultipleBookingHandler(CinemaDbContext db) : ICommandHandler<MultipleBookingCommand, MultipleBookingCommandResult>
{
    public async Task<MultipleBookingCommandResult> Handle(MultipleBookingCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existCustomer = await db.Customers.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
            
            if (!existCustomer) throw new BadRequestException("Customer not found");

            var screenings = await db.Screenings
                .Where(s => request.ScreeningSeats.Select(ss => ss.ScreeningId).Contains(s.Id))
                .ToListAsync(cancellationToken);

            var amount = screenings.Sum(s =>
            {
                var seatCount = request.ScreeningSeats
                    .First(ss => ss.ScreeningId == s.Id)
                    .SeatId.Count();
                return s.Price * seatCount;
            });

            var sale = new Sale
            {
                CustomerId = request.CustomerId,
                SaleDate = DateTime.UtcNow,
                AmountPaid = amount
            };
            
            db.Sales.Add(sale);
            await db.SaveChangesAsync(cancellationToken);

            foreach (var ss in request.ScreeningSeats)
            {
                var (success, message) = await ScreeningHelper.BookScreeningAsync(
                    db, ss.ScreeningId, ss.SeatId.ToArray(), request.CustomerId, sale.Id, cancellationToken);
                
                if (!success)
                {
                    throw new BadRequestException(message);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new MultipleBookingCommandResult(true, "All seats successfully booked", sale.Id);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
