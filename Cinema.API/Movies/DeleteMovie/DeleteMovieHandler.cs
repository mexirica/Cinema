using FluentValidation;

namespace Cinema.API.Movies.DeleteMovie;

/// <summary>
/// Represents a command to delete a movie.
/// </summary>
/// <param name="Id">The ID of the movie to be deleted.</param>
public record DeleteMovieCommand(int Id) : ICommand<DeleteMovieCommandResult>;

/// <summary>
/// Represents the result of deleting a movie.
/// </summary>
/// <param name="Success">Indicates whether the deletion was successful.</param>
public record DeleteMovieCommandResult(bool Success);

/// <summary>
/// Validator for the DeleteMovieCommand.
/// </summary>
public class DeleteMovieCommandValidator : AbstractValidator<DeleteMovieCommand>
{
    public DeleteMovieCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0).WithMessage("Id must be greater than 0");
    }
}

/// <summary>
/// Handler for the DeleteMovieCommand.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class DeleteMovieHandler(CinemaDbContext dbContext) : ICommandHandler<DeleteMovieCommand,DeleteMovieCommandResult>
{
    /// <summary>
    /// Handles the DeleteMovieCommand.
    /// </summary>
    /// <param name="request">The delete movie command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the delete movie command.</returns>
    public async Task<DeleteMovieCommandResult> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var result = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == request.Id,cancellationToken);

        if (result == null)
        {
            throw new NotFoundException("Movie", request.Id);
        }

        dbContext.Movies.Remove(result);
        await dbContext.SaveChangesAsync();

        return new DeleteMovieCommandResult(true);
    }
}