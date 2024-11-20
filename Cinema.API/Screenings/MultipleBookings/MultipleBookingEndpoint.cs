using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.API.Screenings.MultipleBookings;

#region Request and Response

public record ScreeningSeatDto(int ScreeningId, IEnumerable<int> SeatId);

public record MultipleBookingRequest(List<ScreeningSeatDto> ScreeningSeats, int CustomerId);

public record MultipleBookingResponse(bool Success, string Message, int? SaleId);

#endregion

public class MultipleBookingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("screenings/multiple-bookings", async ([FromBody] MultipleBookingRequest req, ISender sender) =>
            {
                var result = await sender.Send(new MultipleBookingCommand(req.ScreeningSeats, req.CustomerId));

                var response = result.Adapt<MultipleBookingResponse>();

                return Results.Ok(response);
            })
            .Produces<MultipleBookingResponse>()
            .ProducesProblem(404)
            .WithSummary("Multiple bookings")
            .WithDescription("Books multiple screenings and/or multiple seats");
    }
}