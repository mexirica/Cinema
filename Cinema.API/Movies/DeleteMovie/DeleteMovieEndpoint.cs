using Mapster;

namespace Cinema.API.Movies.DeleteMovie;

public record DeleteMovieRequest(int Id) : IRequest<bool>;

public record DeleteMovieResponse(bool Success);

public class DeleteMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/movies/{id:int}", async (int id, ISender sender) =>
        {
            var command = new DeleteMovieCommand(id);
            
            var result = await sender.Send(command);

            var response = result.Adapt<DeleteMovieCommandResult>();
            
            return Results.Ok(response);
        }).WithName("Delete Movie")
        .Produces<DeleteMovieCommandResult>()
        .ProducesProblem(400)
        .WithSummary("Delete a movie")
        .WithDescription("Deletes a movie from the database");
    }
}