using Qams.BuildingBlocks.Common;
using Qams.BuildingBlocks.Compliance;
using Qams.ESignature.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ESignatureStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.ESignature.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/esignatures", (HttpRequest request, ESignatureStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var signatures = store.List(context.TenantId);

    return Results.Ok(ApiResponse.Ok(signatures, context.CorrelationId));
});

app.MapPost("/api/v1/esignatures", (CreateESignatureRequest command, HttpRequest request, ESignatureStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ESignatureRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ESignatureRecord>(
            new ApiError("tenant_mismatch", "Request tenant header must match e-signature tenant."),
            context.CorrelationId));
    }

    // TODO: Validate record hash against current record state
    // For now, assume provided hash is correct

    var signature = store.Create(command, Guid.NewGuid(), context.CorrelationId);
    return Results.Created($"/api/v1/esignatures/{signature.SignatureId}", ApiResponse.Ok(signature, context.CorrelationId));
});

app.Run();
