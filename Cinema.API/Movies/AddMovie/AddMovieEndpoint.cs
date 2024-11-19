using Mapster;

namespace Cinema.API.Movies.AddMovie;

/// <summary>
/// Represents a request to add a movie.
/// </summary>
/// <param name="Title">The title of the movie.</param>
/// <param name="Rating">The rating of the movie.</param>
/// <param name="Description">The description of the movie.</param>
/// <param name="ImageUrl">The URL of the movie's image.</param>
/// <param name="Duration">The duration of the movie.</param>
public record AddMovieRequest(string Title, float Rating, string Description, string ImageUrl, TimeSpan Duration);

/// <summary>
/// Represents a response after adding a movie.
/// </summary>
/// <param name="Id">The ID of the added movie.</param>
public record AddMovieResponse(int Id);

/// <summary>
/// Endpoint for adding a movie.
/// </summary>
public class AddMovieEndpoint : ICarterModule
{
    /// <summary>
    /// Adds the routes for the AddMovie endpoint.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("movies", async (ISender mediator, AddMovieRequest request) =>
            {
                // Sends the AddMovieCommand to the mediator and awaits the result.
                var result = await mediator.Send(new AddMovieCommand(request.Title, request.Rating, request.Description, request.ImageUrl, request.Duration));

                // Adapts the result to an AddMovieResponse.
                var response = result.Adapt<AddMovieResponse>();

                // Returns a Created result with the response.
                return Results.Created($"/movies/{response.Id}", response);
            })
            .WithName("Add Movie")
            .Produces<AddMovieResponse>(201)
            .ProducesProblem(400)
            .WithSummary("Add a movie")
            .WithDescription("Adds a movie to the database");
    }
}