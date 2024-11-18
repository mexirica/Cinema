namespace Cinema.API.Movies.DeleteMovie;

public record DeleteMovieCommand(int Id) : ICommand<DeleteMovieCommandResult>;

public record DeleteMovieCommandResult(bool Success);

public class DeleteMovieHandler(CinemaDbContext dbContext) : ICommandHandler<DeleteMovieCommand,DeleteMovieCommandResult>
{
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