using Qams.BuildingBlocks.Common;
using Qams.Reporting.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ReportingStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Reporting.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/report-definitions", (HttpRequest request, ReportingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var definitions = store.ListDefinitions(context.TenantId);
    return Results.Ok(ApiResponse.Ok(definitions, context.CorrelationId));
});

app.MapPost("/api/v1/reports", (GenerateReportRequest command, HttpRequest request, ReportingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ReportResult>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ReportResult>(new ApiError("tenant_mismatch", "Request tenant header must match report tenant."), context.CorrelationId));
    }

    var report = store.Generate(command, context.CorrelationId);
    if (report is null)
    {
        return Results.BadRequest(ApiResponse.Fail<ReportResult>(new ApiError("report_definition_not_found", "Report definition not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(report, context.CorrelationId));
});

app.Run();

public partial class Program { }
