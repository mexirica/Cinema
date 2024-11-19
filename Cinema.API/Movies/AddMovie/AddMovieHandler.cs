using Cinema.API.Models;
using FluentValidation;

namespace Cinema.API.Movies.AddMovie;

/// <summary>
/// Represents a command to add a movie.
/// </summary>
/// <param name="Title">The title of the movie.</param>
/// <param name="Rating">The rating of the movie.</param>
/// <param name="Description">The description of the movie.</param>
/// <param name="ImageUrl">The URL of the movie's image.</param>
/// <param name="Duration">The duration of the movie.</param>
public record AddMovieCommand(string Title, float Rating, string Description, string ImageUrl, TimeSpan Duration) : ICommand<AddMovieCommandResult>;

/// <summary>
/// Represents the result of adding a movie.
/// </summary>
/// <param name="Id">The ID of the added movie.</param>
public record AddMovieCommandResult(int Id);

/// <summary>
/// Validator for the AddMovieCommand.
/// </summary>
public class AddMovieCommandValidator : AbstractValidator<AddMovieCommand>
{
    public AddMovieCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(0, 10);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ImageUrl).NotEmpty();
        RuleFor(x => x.Duration).GreaterThan(TimeSpan.Zero);
    }
}

/// <summary>
/// Handler for the AddMovieCommand.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class AddMovieHandler(CinemaDbContext dbContext) : ICommandHandler<AddMovieCommand, AddMovieCommandResult>
{
    /// <summary>
    /// Handles the AddMovieCommand.
    /// </summary>
    /// <param name="request">The add movie command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the add movie command.</returns>
    public async Task<AddMovieCommandResult> Handle(AddMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Rating = request.Rating,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Duration = request.Duration
        };

        await dbContext.Movies.AddAsync(movie, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddMovieCommandResult(movie.Id);
    }
}