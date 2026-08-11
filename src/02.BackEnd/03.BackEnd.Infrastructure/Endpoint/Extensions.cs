using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;

namespace EBVL.BackEnd.Infrastructure.Endpoint;

public static class Extensions
{
    public static void RegisterEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpoints = assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            _ = endpoint.RegisterTo(app)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .DisableAntiforgery();
        }
    }

    public static OpenApiOptions UseCustomSchemaIds(this OpenApiOptions config, Func<Type, string?> typeSchemaTransformer, bool includeValueTypes = false)
    {
        return config.AddSchemaTransformer((schema, context, _) =>
        {
            if (!includeValueTypes && (context.JsonTypeInfo.Type.IsValueType || context.JsonTypeInfo.Type == typeof(string)))
            {
                return Task.CompletedTask;
            }

            if (schema.Extensions == null || !schema.Extensions.TryGetValue("x-schema-id", out var _))
            {
                return Task.CompletedTask;
            }

            var transformedTypeName = typeSchemaTransformer(context.JsonTypeInfo.Type);
            schema.Title = transformedTypeName;

            return Task.CompletedTask;
        });
    }
}
