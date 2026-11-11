namespace Domain.Entities;

public sealed class TodoItem
{
    private TodoItem()
    {
    }

    public TodoItem(Guid id, string title, bool isCompleted = false)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", nameof(id));
        }

        Id = id;
        Rename(title);
        IsCompleted = isCompleted;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = null;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        title = title.Trim();
        if (title.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));
        }

        Title = title;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void SetCompletion(bool isCompleted)
    {
        IsCompleted = isCompleted;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
