using Cinema.API.Helpers;
using Cinema.API.Models;
using FluentValidation;

namespace Cinema.API.Screenings.MultipleBookings;

#region Command and CommandResult
public record MultipleBookingCommand(IEnumerable<ScreeningSeatDto> ScreeningSeats, int CustomerId)
    : ICommand<MultipleBookingCommandResult>;

public record MultipleBookingCommandResult(bool Success, string Message, int? SaleId);

#endregion

#region Validation

public class MultipleBookingCommandValidator : AbstractValidator<MultipleBookingCommand>
{
    public MultipleBookingCommandValidator()
    {
        RuleForEach(x => x.ScreeningSeats)
            .NotEmpty()
            .SetValidator(new ScreeningSeatDtoValidator());
        
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Customer Id must be greater than 0");
    }
}

public class ScreeningSeatDtoValidator : AbstractValidator<ScreeningSeatDto>
{
    public ScreeningSeatDtoValidator()
    {
        // Valida que todos os SeatIds são maiores que 0
        RuleForEach(x => x.SeatId)
            .GreaterThan(0)
            .WithMessage("Each Seat Id must be greater than 0");

        RuleFor(x => x.ScreeningId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Screening Id must be greater than 0");
    }
}


#endregion

public class MultipleBookingHandler(CinemaDbContext db)
    : ICommandHandler<MultipleBookingCommand, MultipleBookingCommandResult>
{
    public async Task<MultipleBookingCommandResult> Handle(MultipleBookingCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existCustomer = await db.Customers.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);

            if (!existCustomer) throw new NotFoundException("Customer not found");

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

                if (!success) return new MultipleBookingCommandResult(false, message, null);
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