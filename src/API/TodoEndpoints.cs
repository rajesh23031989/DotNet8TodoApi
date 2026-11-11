using Domain.Entities;
using Domain.Repositories;
using Metadata;

namespace API;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/todos")
            .WithTags(ApiMetadata.TodoTag);

        group.MapGet("/", GetAllAsync)
            .WithName("GetTodos")
            .Produces<IReadOnlyCollection<TodoItem>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetTodoById")
            .Produces<TodoItem>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateAsync)
            .WithName("CreateTodo")
            .Produces<TodoItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateTodo")
            .Produces<TodoItem>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteTodo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetAllAsync(
        ITodoRepository repository,
        CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return Results.Ok(items);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ITodoRepository repository,
        CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);
        return item is null ? Results.NotFound() : Results.Ok(item);
    }

    private static async Task<IResult> CreateAsync(
        CreateTodoRequest request,
        ITodoRepository repository,
        CancellationToken cancellationToken)
    {
        var error = ValidateTitle(request.Title);
        if (error is not null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Title)] = [error]
            });
        }

        var item = new TodoItem(Guid.NewGuid(), request.Title!);
        await repository.AddAsync(item, cancellationToken);
        return Results.CreatedAtRoute("GetTodoById", new { id = item.Id }, item);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateTodoRequest request,
        ITodoRepository repository,
        CancellationToken cancellationToken)
    {
        var error = ValidateTitle(request.Title);
        if (error is not null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Title)] = [error]
            });
        }

        var item = await repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return Results.NotFound();
        }

        item.Rename(request.Title!);
        item.SetCompletion(request.IsCompleted);
        return Results.Ok(item);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        ITodoRepository repository,
        CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static string? ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title is required.";
        }

        return title.Trim().Length > 200 ? "Title cannot exceed 200 characters." : null;
    }
}
