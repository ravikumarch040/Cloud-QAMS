using Qams.BuildingBlocks.Common;
using Qams.ProductMaster.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProductMasterStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.ProductMaster.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/products", (HttpRequest request, ProductMasterStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var products = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(products, context.CorrelationId));
});

app.MapGet("/api/v1/products/{productId}", (string productId, HttpRequest request, ProductMasterStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var product = store.Get(productId, context.TenantId);
    if (product is null)
    {
        return Results.NotFound(ApiResponse.Fail<ProductRecord>(new ApiError("product_not_found", "Product not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(product, context.CorrelationId));
});

app.MapPost("/api/v1/products", (CreateProductRequest command, HttpRequest request, ProductMasterStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ProductRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ProductRecord>(new ApiError("tenant_mismatch", "Request tenant header must match product tenant."), context.CorrelationId));
    }

    var product = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/products/{product.ProductId}", ApiResponse.Ok(product, context.CorrelationId));
});

app.Run();

public partial class Program { }
