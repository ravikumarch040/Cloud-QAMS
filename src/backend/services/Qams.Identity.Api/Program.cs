using Qams.BuildingBlocks.Common;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var tenants = new List<TenantSummary>
{
    new("tenant-life-sciences-demo", "Life Sciences Demo", "ValidatedCore", "Active"),
    new("tenant-medtech-demo", "Medtech Demo", "EnterpriseValidated", "Active")
};

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Identity.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/admin/tenants", (HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    return Results.Ok(ApiResponse.Ok<IReadOnlyCollection<TenantSummary>>(tenants, context.CorrelationId));
});

app.MapPost("/api/v1/admin/tenants", (CreateTenantRequest command, HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<TenantSummary>(headerError, context.CorrelationId));
    }

    var tenant = new TenantSummary(
        $"tenant-{command.Slug}",
        command.DisplayName,
        command.ValidationTier,
        "Provisioning");

    tenants.Add(tenant);
    return Results.Created($"/api/v1/admin/tenants/{tenant.TenantId}", ApiResponse.Ok(tenant, context.CorrelationId));
});

app.Run();

sealed record TenantSummary(string TenantId, string DisplayName, string ValidationTier, string Status);
sealed record CreateTenantRequest(string Slug, string DisplayName, string ValidationTier);

public partial class Program { }
