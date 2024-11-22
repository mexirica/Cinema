using BuildingBlocks.MessageBus;
using Cinema.API.Booking.Helpers;
using Cinema.API.Helpers;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore.Storage;

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

public class MultipleBookingHandler(CinemaDbContext db, IPublishEndpoint publisher)
		: ICommandHandler<MultipleBookingCommand, MultipleBookingCommandResult>
{
public async Task<MultipleBookingCommandResult> Handle(MultipleBookingCommand request, CancellationToken cancellationToken)
{
    await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
    try
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer is null) throw new NotFoundException("Customer not found");

        var screenings = await db.Screenings
            .Include(s => s.Movie)
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
            var (success, message, saleScreenings, saleScreeningSeats) = await ScreeningHelper.BookScreeningAsync(
                db, ss.ScreeningId, ss.SeatId.ToArray(), sale.Id, cancellationToken);

            if (!success)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new MultipleBookingCommandResult(false, message, null);
            }

            db.SaleScreenings.AddRange(saleScreenings);
            db.SaleScreeningSeats.AddRange(saleScreeningSeats);
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await publisher.Publish(
            MessageFactory.CreateTicketPurchasedMessage(customer, screenings), cancellationToken);

        return new MultipleBookingCommandResult(true, "All seats successfully booked", sale.Id);
    }
    catch
    {
        if (transaction.GetDbTransaction().Connection != null)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        throw;
    }
}

}