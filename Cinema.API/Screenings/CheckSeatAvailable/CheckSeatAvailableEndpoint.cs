namespace Cinema.API.Screenings.CheckSeatAvailable;
public record CheckSeatAvailableResponse(bool IsSeatAvailable, string Message);

public class CheckSeatAvailableEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("screenings/{screeningId:int}/check-seat-available/{seatId:int}", async (int screeningId, int seatId, ISender sender) =>
            {
                var result = await sender.Send(new CheckSeatAvailableQuery(screeningId, seatId));
                
                return Results.Ok(result);
            })
            .Produces<CheckSeatAvailableResponse>()
            .ProducesProblem(404)
            .WithSummary("Check if a seat is available")
            .WithDescription("Checks if a seat is available for a screening");
    }
}