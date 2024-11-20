using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.API.Screenings.ChooseSeat;

public record ChooseSeatRequest(int SeatId, int CustomerId);

public record ChooseSeatResponse(bool Success, string Message);

public class BookScreeningSeatEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("screenings/{screeningId:int}/choose-seat",
                async (int screeningId, [FromBody] ChooseSeatRequest req, ISender sender) =>
                {
                    var result = await sender.Send(new ChooseSeatCommand(screeningId, req.SeatId, req.CustomerId));

                    var response = result.Adapt<ChooseSeatResponse>();

                    return Results.Ok(response);
                })
            .Produces<ChooseSeatResponse>()
            .ProducesProblem(404)
            .WithSummary("Choose a seat")
            .WithDescription("Chooses a seat for a screening");
    }
}