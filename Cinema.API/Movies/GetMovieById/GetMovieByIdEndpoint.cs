

namespace Cinema.API.Movies.GetMovieById;

public class GetMovieByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{id:int}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetMovieByIdQuery(id));

                return Results.Ok(result);
            }).WithName("Get Movie By Id")
            .Produces<GetMovieByIdQueryResult>()
            .ProducesProblem(404)
            .WithSummary("Get a movie by its id")
            .WithDescription("Returns a movie by its id");
    }
}