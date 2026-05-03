using Qams.BuildingBlocks.Common;
using Qams.Complaints.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ComplaintsStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Complaints.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/complaints", (HttpRequest request, ComplaintsStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var complaints = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(complaints, context.CorrelationId));
});

app.MapGet("/api/v1/complaints/{complaintId}", (string complaintId, HttpRequest request, ComplaintsStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var complaint = store.Get(complaintId, context.TenantId);
    if (complaint is null)
    {
        return Results.NotFound(ApiResponse.Fail<ComplaintRecord>(new ApiError("complaint_not_found", "Complaint not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(complaint, context.CorrelationId));
});

app.MapPost("/api/v1/complaints", (CreateComplaintRequest command, HttpRequest request, ComplaintsStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ComplaintRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ComplaintRecord>(new ApiError("tenant_mismatch", "Request tenant header must match complaint tenant."), context.CorrelationId));
    }

    var created = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/complaints/{created.ComplaintId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/complaints/{complaintId}/resolve", (string complaintId, ResolveComplaintRequest command, HttpRequest request, ComplaintsStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ComplaintRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ComplaintRecord>(new ApiError("tenant_mismatch", "Request tenant header must match complaint tenant."), context.CorrelationId));
    }

    var resolved = store.Resolve(complaintId, command, context.TenantId, context.CorrelationId);
    if (resolved is null)
    {
        return Results.NotFound(ApiResponse.Fail<ComplaintRecord>(new ApiError("complaint_not_found", "Complaint not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(resolved, context.CorrelationId));
});

app.Run();
