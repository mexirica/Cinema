namespace Cinema.API.Movies.GetMovieById;

/// <summary>
/// Endpoint for getting a movie by its ID.
/// </summary>
public class GetMovieByIdEndpoint : ICarterModule
{
    /// <summary>
    /// Adds the routes for the GetMovieById endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{id:int}", async (int id, ISender sender) =>
            {
                // Sends the GetMovieByIdQuery to the sender and awaits the result.
                var result = await sender.Send(new GetMovieByIdQuery(id));

                // Returns an Ok result with the query result.
                return Results.Ok(result);
            })
            .WithName("Get Movie By Id")
            .Produces<GetMovieByIdQueryResult>()
            .ProducesProblem(404)
            .WithSummary("Get a movie by its id")
            .WithDescription("Returns a movie by its id");
    }
}