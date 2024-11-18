using Carter;
using MediatR;

namespace Cinema.API.Movies.GetMovieByName;

public class GetMovieByNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{name}", async (string name, ISender sender) =>
        {
            var result = await sender.Send(new GetMovieByNameQuery(name));

            return Results.Ok(result);
        }).WithName("Get Movie By Name")
        .Produces<GetMovieByNameQueryResult>()
        .ProducesProblem(404)
        .WithSummary("Get a movie by its name")
        .WithDescription("Returns a movie by its name");
    }
}