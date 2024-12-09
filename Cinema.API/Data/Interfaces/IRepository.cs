namespace Cinema.API.Data;

public interface IRepository<T> where T : class
{
    Task<T> GetByID(int id,CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default);
    Task<T> Add(T entity, CancellationToken cancellationToken = default);
    Task<T> Update(T entity, CancellationToken cancellationToken = default);
    Task<T> Delete(int id, CancellationToken cancellationToken = default);
}