using Mapster;

namespace Cinema.API.Movies.AddMovie;

public record AddMovieRequest(string Title, float Rating, string Description, string ImageUrl, TimeSpan Duration);

public record AddMovieResponse(int Id);

public class AddMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("movies", async (ISender mediator, AddMovieRequest request) =>
        {
            var result = await mediator.Send(new AddMovieCommand(request.Title, request.Rating, request.Description, request.ImageUrl, request.Duration));

            var response = result.Adapt<AddMovieResponse>();
            
            return Results.Created($"/movies/{response.Id}", result);
        }).WithName("Add Movie")
        .Produces<AddMovieResponse>(201)
        .ProducesProblem(400)
        .WithSummary("Add a movie")
        .WithDescription("Adds a movie to the database");
    }
}