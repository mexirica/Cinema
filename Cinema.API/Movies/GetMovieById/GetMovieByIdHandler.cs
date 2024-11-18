namespace Cinema.API.Movies.GetMovieById;

public record GetMovieByIdQuery(int MovieId) : IQuery<GetMovieByIdQueryResult>;

public record GetMovieByIdQueryResult(string Title, string Description, string ImageUrl, float Rating, TimeSpan Duration);

public class GetMovieByIdHandler(CinemaDbContext dbContext) : IQueryHandler<GetMovieByIdQuery,GetMovieByIdQueryResult>
{
    public async Task<GetMovieByIdQueryResult> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        var movie = await dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.MovieId);
        
        if (movie == null)
        {
            throw new NotFoundException("Movie", request.MovieId);
        }
        
        return new GetMovieByIdQueryResult(movie.Title, movie.Description, movie.ImageUrl, movie.Rating, movie.Duration);
    }
}