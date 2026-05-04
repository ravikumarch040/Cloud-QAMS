using Qams.BuildingBlocks.Common;
using Qams.RiskDesignControls.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<RiskDesignControlsStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.RiskDesignControls.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/risk-assessments", (HttpRequest request, RiskDesignControlsStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var assessments = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(assessments, context.CorrelationId));
});

app.MapPost("/api/v1/risk-assessments", (CreateRiskAssessmentRequest command, HttpRequest request, RiskDesignControlsStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<RiskAssessmentRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<RiskAssessmentRecord>(new ApiError("tenant_mismatch", "Request tenant header must match assessment tenant."), context.CorrelationId));
    }

    var assessment = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/risk-assessments/{assessment.RiskAssessmentId}", ApiResponse.Ok(assessment, context.CorrelationId));
});

app.Run();

public partial class Program { }
