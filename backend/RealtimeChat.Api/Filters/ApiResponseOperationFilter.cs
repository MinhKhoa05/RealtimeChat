using Microsoft.OpenApi.Models;
using RealtimeChat.Api.Responses;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealtimeChat.Api.Filters;

public sealed class ApiResponseOperationFilter : IOperationFilter
{
    private static readonly HashSet<string> BodylessStatusCodes = ["204", "304"];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (statusCode, response) in operation.Responses)
        {
            if (BodylessStatusCodes.Contains(statusCode))
                continue;

            if (response.Content.Count == 0)
                continue;

            // Ưu tiên application/json nếu đã tồn tại.
            // Nếu chưa có, lấy schema từ content hiện có.
            var jsonContent = response.Content.TryGetValue(
                "application/json", out var existingJson)
                    ? existingJson
                    : response.Content.Values.FirstOrDefault();

            if (jsonContent is null)
                continue;

            // Chuẩn hóa media type trong OpenAPI.
            response.Content.Clear();
            response.Content["application/json"] = jsonContent;

            var dataSchema = jsonContent.Schema;

            if (IsApiResponseSchema(dataSchema))
                continue;

            // Sinh schema từ model ApiResponse hiện có.
            var generatedSchema = context.SchemaGenerator.GenerateSchema(
                typeof(ApiResponse),
                context.SchemaRepository);

            var responseSchema = generatedSchema;

            // Nếu schema là $ref, lấy schema thực từ repository.
            if (generatedSchema.Reference is not null)
            {
                var schemaId = generatedSchema.Reference.Id;

                if (!context.SchemaRepository.Schemas.TryGetValue(
                        schemaId, out responseSchema))
                {
                    continue;
                }
            }

            var properties = new Dictionary<string, OpenApiSchema>(responseSchema.Properties);

            // Giữ Success, Message; thay Data bằng schema riêng
            // của endpoint hiện tại.
            properties["data"] = dataSchema;

            jsonContent.Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = properties,
                Required = responseSchema.Required is null
                    ? new HashSet<string>()
                    : new HashSet<string>(responseSchema.Required)
            };
        }
    }

    private static bool IsApiResponseSchema(OpenApiSchema schema)
    {
        if (schema.Reference?.Id == nameof(ApiResponse))
            return true;

        return schema.Properties is not null
            && schema.Properties.ContainsKey("success")
            && schema.Properties.ContainsKey("message")
            && schema.Properties.ContainsKey("data");
    }
}
