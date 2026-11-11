using Domain.Entities;

namespace Domain.Repositories;

public interface ITodoRepository
{
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(TodoItem item, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
