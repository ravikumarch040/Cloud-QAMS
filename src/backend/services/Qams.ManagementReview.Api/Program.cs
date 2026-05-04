using Qams.BuildingBlocks.Common;
using Qams.ManagementReview.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ManagementReviewStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.ManagementReview.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/review-packs", (HttpRequest request, ManagementReviewStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var packs = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(packs, context.CorrelationId));
});

app.MapGet("/api/v1/review-packs/{reviewPackId}", (string reviewPackId, HttpRequest request, ManagementReviewStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var pack = store.Get(reviewPackId, context.TenantId);
    if (pack is null)
    {
        return Results.NotFound(ApiResponse.Fail<ReviewPackRecord>(new ApiError("review_pack_not_found", "Review pack not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(pack, context.CorrelationId));
});

app.MapPost("/api/v1/review-packs", (CreateReviewPackRequest command, HttpRequest request, ManagementReviewStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ReviewPackRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ReviewPackRecord>(new ApiError("tenant_mismatch", "Request tenant header must match review pack tenant."), context.CorrelationId));
    }

    var pack = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/review-packs/{pack.ReviewPackId}", ApiResponse.Ok(pack, context.CorrelationId));
});

app.Run();

public partial class Program { }
