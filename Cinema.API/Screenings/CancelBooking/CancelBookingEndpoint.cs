using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.API.Screenings.CancelBooking;

public record CancelBookingRequest(int SaleId, int CustomerId);

public record CancelBookingResponse(bool Success, string? Error);

public class CancelBookingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("screenings/{screeningId:int}/cancel-booking",
                async (int screeningId, [FromBody] CancelBookingRequest req, ISender sender) =>
                {
                    var result = await sender.Send(new CancelBookingCommand(screeningId, req.SaleId, req.CustomerId));

                    var response = result.Adapt<CancelBookingResponse>();

                    return Results.Ok(response);
                })
            .Produces<CancelBookingResponse>()
            .ProducesProblem(404)
            .WithSummary("Cancel a booking")
            .WithDescription("Cancels a booking for a screening");
    }
}