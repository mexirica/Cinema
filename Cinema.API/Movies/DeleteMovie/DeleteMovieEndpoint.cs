using Mapster;

namespace Cinema.API.Movies.DeleteMovie;

/// <summary>
/// Represents a response after deleting a movie.
/// </summary>
/// <param name="Success">Indicates whether the deletion was successful.</param>
public record DeleteMovieResponse(bool Success);

/// <summary>
/// Endpoint for deleting a movie.
/// </summary>
public class DeleteMovieEndpoint : ICarterModule
{
    /// <summary>
    /// Adds the routes for the DeleteMovie endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/movies/{id:int}", async (int id, ISender sender) =>
            {
                // Creates a new DeleteMovieCommand with the provided id.
                var command = new DeleteMovieCommand(id);

                // Sends the DeleteMovieCommand to the sender and awaits the result.
                var result = await sender.Send(command);

                // Adapts the result to a DeleteMovieResponse.
                var response = result.Adapt<DeleteMovieResponse>();

                // Returns an Ok result with the response.
                return Results.Ok(response);
            })
            .WithName("Delete Movie")
            .Produces<DeleteMovieCommandResult>()
            .ProducesProblem(400)
            .WithSummary("Delete a movie")
            .WithDescription("Deletes a movie from the database");
    }
}