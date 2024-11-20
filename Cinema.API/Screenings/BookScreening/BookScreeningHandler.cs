using Cinema.API.Helpers;
using Cinema.API.Models;

namespace Cinema.API.Screenings.BookScreening;

public record BuyScreeningCommand(int ScreeningId,int CustomerId) : ICommand<BuyScreeningResult>;

public record BuyScreeningResult(bool Success, int? SaleId, string? Message);

public class BookScreeningHandler(CinemaDbContext db) : ICommandHandler<BuyScreeningCommand,BuyScreeningResult>
{
    public async Task<BuyScreeningResult> Handle(BuyScreeningCommand request, CancellationToken cancellationToken)
    {
        var screening = await ScreeningHelper.GetScreeningAsync(db, request.ScreeningId, cancellationToken);
        
        var existCustomer = await db.Customers.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
        
        if (!existCustomer) throw new BadRequestException("Customer not found");

        var totalAvailableSeats = screening.Room.RoomSeat.Count - await db.SaleScreenings
            .CountAsync(ss => ss.ScreeningId == request.ScreeningId, cancellationToken);

        if (totalAvailableSeats <= 0)
            return new BuyScreeningResult(false,null,"No available seats");

        var sale = new Sale
        {
            CustomerId = request.CustomerId,
            SaleDate = DateTime.UtcNow,
            AmountPaid = screening.Price
        };

        db.Sales.Add(sale);
        await db.SaveChangesAsync(cancellationToken);

        var saleScreening = new SaleScreening
        {
            ScreeningId = request.ScreeningId,
            SaleId = sale.Id,
            UnassignedSeat = true
        };

        db.SaleScreenings.Add(saleScreening);
        await db.SaveChangesAsync(cancellationToken);
        
        return new BuyScreeningResult(true,sale.Id,null);
    }

}