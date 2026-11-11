using System.Collections.Concurrent;
using Domain.Entities;
using Domain.Repositories;

namespace Data.Repositories;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    public Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<TodoItem> result = _items.Values
            .OrderBy(item => item.CreatedAtUtc)
            .ToArray();
        return Task.FromResult(result);
    }

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task AddAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException($"A todo item with id '{item.Id}' already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.TryRemove(id, out _));
    }
}
