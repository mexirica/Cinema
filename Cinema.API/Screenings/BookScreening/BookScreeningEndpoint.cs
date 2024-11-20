using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.API.Screenings.BookScreening;

public record BuyScreeningRequest(int CustomerId);

public record BuyScreeningResponse(bool Success, int? SaleId ,string? Message);

public class BookScreeningEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("screenings/{screeningId:int}/buy", async (int screeningId,[FromBody] BuyScreeningRequest req ,ISender sender) =>
            {
                var result = await sender.Send(new BuyScreeningCommand(screeningId,req.CustomerId));

                var response = result.Adapt<BuyScreeningResponse>();

                return Results.Ok(response);
            })
            .Produces<BuyScreeningResponse>()
            .ProducesProblem(404)
            .WithSummary("Buy a screening")
            .WithDescription("Buys a screening for a customer");
    }
}