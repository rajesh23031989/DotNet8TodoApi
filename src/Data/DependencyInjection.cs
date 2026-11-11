using Data.Repositories;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        return services;
    }
}
