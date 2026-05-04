using Qams.BuildingBlocks.Common;
using Qams.Rules.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<RuleStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Rules.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/rules", (HttpRequest request, RuleStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var rules = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(rules, context.CorrelationId));
});

app.MapPost("/api/v1/rules", (CreateRuleRequest command, HttpRequest request, RuleStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<RuleDefinition>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<RuleDefinition>(new ApiError("tenant_mismatch", "Request tenant header must match rule tenant."), context.CorrelationId));
    }

    var rule = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/rules/{rule.RuleId}", ApiResponse.Ok(rule, context.CorrelationId));
});

app.MapPost("/api/v1/rules/evaluate", (RuleEvaluationRequest requestBody, HttpRequest request, RuleStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<RuleEvaluationResult>(headerError, context.CorrelationId));
    }

    if (!string.Equals(requestBody.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<RuleEvaluationResult>(new ApiError("tenant_mismatch", "Request tenant header must match evaluation tenant."), context.CorrelationId));
    }

    var evaluation = store.Evaluate(requestBody);
    return Results.Ok(ApiResponse.Ok(evaluation, context.CorrelationId));
});

app.MapPost("/api/v1/rules/trigger", (RuleTriggerRequest requestBody, HttpRequest request, RuleStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<RuleTriggerResult>(headerError, context.CorrelationId));
    }

    if (!string.Equals(requestBody.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<RuleTriggerResult>(new ApiError("tenant_mismatch", "Request tenant header must match trigger tenant."), context.CorrelationId));
    }

    var result = store.Trigger(requestBody);
    return Results.Ok(ApiResponse.Ok(result, context.CorrelationId));
});

app.Run();

public partial class Program { }
