using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace EBVL.BackEnd.Infrastructure.Endpoint;

public sealed class CustomSchemaTransformer : IOpenApiSchemaTransformer
{
    private const string MetadataKeyForSchemaId = "x-schema-id";

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(decimal))
        {
            schema.Format = "decimal";

            return Task.CompletedTask;
        }

        if (context.JsonTypeInfo.Type.IsValueType || context.JsonTypeInfo.Type == typeof(string))
        {
            return Task.CompletedTask;
        }

        var fullName = context.JsonTypeInfo.Type.FullName;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Task.CompletedTask;
        }

        if (fullName.Contains('['))
        {
            return Task.CompletedTask;
        }

        var schemaId = fullName.Replace("+", ".", StringComparison.Ordinal);

        if (schema.Metadata is null)
        {
            schema.Metadata = new Dictionary<string, object>
            {
                { MetadataKeyForSchemaId, schemaId }
            };

            return Task.CompletedTask;
        }

        schema.Metadata[MetadataKeyForSchemaId] = schemaId;

        return Task.CompletedTask;
    }
}
