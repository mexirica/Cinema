namespace Cinema.API.Movies.GetMovieByName;

public record GetMovieByNameQuery(string MovieName) : IQuery<GetMovieByNameQueryResult>;
    
public record GetMovieByNameQueryResult(string Title, string Description, string ImageUrl, float Rating , TimeSpan Duration);
    

public class GetMovieByNameQueryHandler(CinemaDbContext dbContext) : IQueryHandler<GetMovieByNameQuery,GetMovieByNameQueryResult>
{
    public async Task<GetMovieByNameQueryResult> Handle(GetMovieByNameQuery request, CancellationToken cancellationToken)
    {
        var movie = await dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Title == request.MovieName);
        
        if (movie == null)
        {
            throw new NotFoundException("Movie", request.MovieName);
        }
        
        return new GetMovieByNameQueryResult(movie.Title, movie.Description, movie.ImageUrl, movie.Rating, movie.Duration);
    }
}