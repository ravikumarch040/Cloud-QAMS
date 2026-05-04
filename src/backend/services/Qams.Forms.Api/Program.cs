using Qams.BuildingBlocks.Common;
using Qams.Forms.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FormStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Forms.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/form-definitions", (HttpRequest request, FormStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var definitions = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(definitions, context.CorrelationId));
});

app.MapGet("/api/v1/form-definitions/{formDefinitionId}", (string formDefinitionId, HttpRequest request, FormStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var definition = store.Get(formDefinitionId, context.TenantId);
    if (definition is null)
    {
        return Results.NotFound(ApiResponse.Fail<FormDefinition>(new ApiError("form_not_found", "Form definition not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(definition, context.CorrelationId));
});

app.MapPost("/api/v1/form-definitions", (CreateFormDefinitionRequest command, HttpRequest request, FormStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<FormDefinition>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<FormDefinition>(new ApiError("tenant_mismatch", "Request tenant header must match form tenant."), context.CorrelationId));
    }

    var created = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/form-definitions/{created.FormDefinitionId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/form-definitions/{formDefinitionId}/validate", (string formDefinitionId, ValidateFormSubmissionRequest command, HttpRequest request, FormStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<FormValidationResult>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<FormValidationResult>(new ApiError("tenant_mismatch", "Request tenant header must match form tenant."), context.CorrelationId));
    }

    var result = store.Validate(formDefinitionId, command, context.TenantId);
    if (result is null)
    {
        return Results.NotFound(ApiResponse.Fail<FormValidationResult>(new ApiError("form_not_found", "Form definition not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(result, context.CorrelationId));
});

app.Run();

public partial class Program { }
