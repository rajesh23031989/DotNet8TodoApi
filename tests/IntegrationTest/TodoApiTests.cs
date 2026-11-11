using System.Net;
using System.Net.Http.Json;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTest;

public sealed class TodoApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Todo_CanBeCreatedReadUpdatedAndDeleted()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new
        {
            title = "Create a .NET 8 API"
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", new
        {
            title = "Create and test a .NET 8 API",
            isCompleted = true
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(updated);
        Assert.True(updated.IsCompleted);

        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithBlankTitle_ReturnsValidationProblem()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new
        {
            title = "   "
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
