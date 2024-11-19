namespace Cinema.API.Movies.GetMovieByName;

/// <summary>
/// Represents a query to get a movie by its name.
/// </summary>
/// <param name="MovieName">The name of the movie.</param>
public record GetMovieByNameQuery(string MovieName) : IQuery<GetMovieByNameQueryResult>;

/// <summary>
/// Represents the result of getting a movie by its name.
/// </summary>
/// <param name="Title">The title of the movie.</param>
/// <param name="Description">The description of the movie.</param>
/// <param name="ImageUrl">The URL of the movie's image.</param>
/// <param name="Rating">The rating of the movie.</param>
/// <param name="Duration">The duration of the movie.</param>
public record GetMovieByNameQueryResult(string Title, string Description, string ImageUrl, float Rating, TimeSpan Duration);

/// <summary>
/// Handler for the GetMovieByNameQuery.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class GetMovieByNameQueryHandler(CinemaDbContext dbContext) : IQueryHandler<GetMovieByNameQuery, GetMovieByNameQueryResult>
{
    /// <summary>
    /// Handles the GetMovieByNameQuery.
    /// </summary>
    /// <param name="request">The get movie by name query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the get movie by name query.</returns>
    /// <exception cref="NotFoundException">Thrown when the movie is not found.</exception>
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