using Qams.BuildingBlocks.Common;
using Qams.Policy.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<PolicyStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Policy.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapPost("/api/v1/policy/evaluate", (PolicyEvaluationRequest request, HttpRequest httpRequest) =>
{
    var context = QamsRequestContextFactory.From(httpRequest);
    var headerError = QamsRequestContextFactory.Validate(httpRequest);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<PolicyEvaluationResult>(headerError, context.CorrelationId));
    }

    var store = app.Services.GetRequiredService<PolicyStore>();
    var result = store.Evaluate(request, context.TenantId);

    return Results.Ok(ApiResponse.Ok(result, context.CorrelationId));
});

app.Run();
