namespace Cinema.API.Movies.GetMovieById;

/// <summary>
/// Represents a query to get a movie by its ID.
/// </summary>
/// <param name="MovieId">The ID of the movie.</param>
public record GetMovieByIdQuery(int MovieId) : IQuery<GetMovieByIdQueryResult>;

/// <summary>
/// Represents the result of getting a movie by its ID.
/// </summary>
/// <param name="Title">The title of the movie.</param>
/// <param name="Description">The description of the movie.</param>
/// <param name="ImageUrl">The URL of the movie's image.</param>
/// <param name="Rating">The rating of the movie.</param>
/// <param name="Duration">The duration of the movie.</param>
public record GetMovieByIdQueryResult(string Title, string Description, string ImageUrl, float Rating, TimeSpan Duration);

/// <summary>
/// Handler for the GetMovieByIdQuery.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class GetMovieByIdHandler(CinemaDbContext dbContext) : IQueryHandler<GetMovieByIdQuery, GetMovieByIdQueryResult>
{
    /// <summary>
    /// Handles the GetMovieByIdQuery.
    /// </summary>
    /// <param name="request">The get movie by ID query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the get movie by ID query.</returns>
    /// <exception cref="NotFoundException">Thrown when the movie is not found.</exception>
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