using API;
using Data;
using Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddData();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
    {
        name = ApiMetadata.Name,
        description = ApiMetadata.Description,
        version = ApiMetadata.Version
    }))
    .WithName("GetApiInfo");

app.MapHealthChecks("/health").WithTags(ApiMetadata.HealthTag);
app.MapTodoEndpoints();

app.Run();

public partial class Program
{
}
