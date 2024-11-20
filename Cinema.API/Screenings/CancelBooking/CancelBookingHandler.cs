using FluentValidation;

namespace Cinema.API.Screenings.CancelBooking;

#region Command and Result

public record CancelBookingCommand(int ScreeningId, int SaleId, int CustomerId) : ICommand<CancelBookingResult>;

public record CancelBookingResult(bool Success, string? Error);

#endregion

#region Validation

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.ScreeningId).NotEmpty().GreaterThan(0).WithMessage("Screening Id must be greater than 0");
        RuleFor(x => x.CustomerId).NotEmpty().GreaterThan(0).WithMessage("Customer Id must be greater than 0");
    }
}

#endregion
public class CancelBookingHandler(CinemaDbContext db) : ICommandHandler<CancelBookingCommand, CancelBookingResult>
{
    public async Task<CancelBookingResult> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var existCustomer = await db.Customers.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!existCustomer) throw new NotFoundException("Customer not found");

        var sale = await db.SaleScreenings
            .Include(ss => ss.Sale)
            .Include(saleScreening => saleScreening.Screening)
            .FirstOrDefaultAsync(
                ss => ss.SaleId == request.SaleId && ss.Sale.CustomerId == request.CustomerId &&
                      ss.ScreeningId == request.ScreeningId, cancellationToken);

        if (sale is null) throw new NotFoundException("Booking not found");

        if (sale.Screening.IsAlreadyPassed()) return new CancelBookingResult(false, "Cannot cancel past bookings");

        db.SaleScreenings.Remove(sale);

        var saleScreeningSeats =
            await db.SaleScreeningSeats.FirstOrDefaultAsync(ss => ss.SaleScreeningId == sale.Id, cancellationToken);

        if (saleScreeningSeats is not null) db.SaleScreeningSeats.Remove(saleScreeningSeats);

        await db.SaveChangesAsync(cancellationToken);

        return new CancelBookingResult(true, null);
    }
}