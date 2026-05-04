using Qams.BuildingBlocks.Common;
using Qams.SupplierQuality.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SupplierQualityStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.SupplierQuality.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/suppliers", (HttpRequest request, SupplierQualityStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var suppliers = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(suppliers, context.CorrelationId));
});

app.MapGet("/api/v1/suppliers/{supplierId}", (string supplierId, HttpRequest request, SupplierQualityStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var supplier = store.Get(supplierId, context.TenantId);
    if (supplier is null)
    {
        return Results.NotFound(ApiResponse.Fail<SupplierRecord>(new ApiError("supplier_not_found", "Supplier not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(supplier, context.CorrelationId));
});

app.MapPost("/api/v1/suppliers", (CreateSupplierRequest command, HttpRequest request, SupplierQualityStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<SupplierRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<SupplierRecord>(new ApiError("tenant_mismatch", "Request tenant header must match supplier tenant."), context.CorrelationId));
    }

    var supplier = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/suppliers/{supplier.SupplierId}", ApiResponse.Ok(supplier, context.CorrelationId));
});

app.MapPost("/api/v1/suppliers/{supplierId}/score", (string supplierId, UpdateSupplierScoreRequest command, HttpRequest request, SupplierQualityStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<SupplierRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<SupplierRecord>(new ApiError("tenant_mismatch", "Request tenant header must match supplier tenant."), context.CorrelationId));
    }

    var updated = store.UpdateScore(supplierId, command, context.TenantId, context.CorrelationId);
    if (updated is null)
    {
        return Results.NotFound(ApiResponse.Fail<SupplierRecord>(new ApiError("supplier_not_found", "Supplier not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(updated, context.CorrelationId));
});

app.Run();

public partial class Program { }
