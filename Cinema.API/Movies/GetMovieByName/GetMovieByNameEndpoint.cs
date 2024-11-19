using Carter;
using MediatR;

namespace Cinema.API.Movies.GetMovieByName;

/// <summary>
/// Endpoint for getting a movie by its name.
/// </summary>
public class GetMovieByNameEndpoint : ICarterModule
{
    /// <summary>
    /// Adds the routes for the GetMovieByName endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{name}", async (string name, ISender sender) =>
            {
                // Sends the GetMovieByNameQuery to the sender and awaits the result.
                var result = await sender.Send(new GetMovieByNameQuery(name));

                // Returns an Ok result with the query result.
                return Results.Ok(result);
            })
            .WithName("Get Movie By Name")
            .Produces<GetMovieByNameQueryResult>()
            .ProducesProblem(404)
            .WithSummary("Get a movie by its name")
            .WithDescription("Returns a movie by its name");
    }
}