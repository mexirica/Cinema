using Cinema.API.Helpers;
using Cinema.API.Models;

namespace Cinema.API.Screenings.ChooseSeat;

public record ChooseSeatCommand(int ScreeningId, int SeatId, int CustomerId) : ICommand<ChooseSeatCommandResult>;

public record ChooseSeatCommandResult(bool Success, string Message, int? SaleId);

public class BookScreeningSeatHandler(CinemaDbContext db) : ICommandHandler<ChooseSeatCommand,ChooseSeatCommandResult>
{
    public async Task<ChooseSeatCommandResult> Handle(ChooseSeatCommand request, CancellationToken cancellationToken)
    {
        var screening = await ScreeningHelper.GetScreeningAsync(db, request.ScreeningId, cancellationToken);

        var seat = await SeatHelper.GetRoomSeatAsync(db, request.SeatId, cancellationToken);

        var isSeatReserved = await SeatHelper.IsSeatReservedAsync(db, screening.Id, seat.SeatId, cancellationToken);

        if (isSeatReserved) return new ChooseSeatCommandResult(false, "Seat already taken",null);
        
        var existCustomer = await db.Customers.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
        
        if (!existCustomer) throw new BadRequestException("Customer not found");
        
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
            UnassignedSeat = false
        };

        db.SaleScreenings.Add(saleScreening);
        await db.SaveChangesAsync(cancellationToken);

        var saleScreeningSeat = new SaleScreeningSeat
        {
            SaleScreening = saleScreening,
            SeatId = request.SeatId
        };

        db.SaleScreeningSeats.Add(saleScreeningSeat);
        await db.SaveChangesAsync(cancellationToken);

        return new ChooseSeatCommandResult(true, "Seat successfully booked", sale.Id);
    }

}