using System.Text.Json.Serialization;
using Qams.BuildingBlocks.Common;
using Qams.DocumentControl.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DocumentControlStore>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.DocumentControl.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/documents", (HttpRequest request, DocumentControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var documents = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(documents, context.CorrelationId));
});

app.MapGet("/api/v1/documents/{documentId}", (string documentId, HttpRequest request, DocumentControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var document = store.Get(documentId, context.TenantId);
    if (document is null)
    {
        return Results.NotFound(ApiResponse.Fail<DocumentControlRecord>(new ApiError("document_not_found", "Document not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(document, context.CorrelationId));
});

app.MapPost("/api/v1/documents", (CreateDocumentRequest command, HttpRequest request, DocumentControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<DocumentControlRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<DocumentControlRecord>(new ApiError("tenant_mismatch", "Request tenant header must match document tenant."), context.CorrelationId));
    }

    var created = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/documents/{created.DocumentId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/documents/{documentId}/versions", (string documentId, AddDocumentVersionRequest command, HttpRequest request, DocumentControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<DocumentControlRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<DocumentControlRecord>(new ApiError("tenant_mismatch", "Request tenant header must match document tenant."), context.CorrelationId));
    }

    var updated = store.AddVersion(documentId, command, context.TenantId, context.CorrelationId);
    if (updated is null)
    {
        return Results.NotFound(ApiResponse.Fail<DocumentControlRecord>(new ApiError("document_not_found", "Document not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(updated, context.CorrelationId));
});

app.MapPost("/api/v1/documents/{documentId}/approve", (string documentId, ApproveDocumentRequest command, HttpRequest request, DocumentControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<DocumentControlRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<DocumentControlRecord>(new ApiError("tenant_mismatch", "Request tenant header must match document tenant."), context.CorrelationId));
    }

    var approved = store.Approve(documentId, command, context.TenantId, context.CorrelationId);
    if (approved is null)
    {
        return Results.NotFound(ApiResponse.Fail<DocumentControlRecord>(new ApiError("document_not_found", "Document not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(approved, context.CorrelationId));
});

app.Run();

public partial class Program { }
