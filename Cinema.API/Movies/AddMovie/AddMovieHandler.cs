using Cinema.API.Models;

namespace Cinema.API.Movies.AddMovie;

public record AddMovieCommand(string Title, float Rating, string Description, string ImageUrl, TimeSpan Duration) : ICommand<AddMovieCommandResult>;

public record AddMovieCommandResult(int Id);

public class AddMovieHandler(CinemaDbContext dbContext) : ICommandHandler<AddMovieCommand,AddMovieCommandResult>
{
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