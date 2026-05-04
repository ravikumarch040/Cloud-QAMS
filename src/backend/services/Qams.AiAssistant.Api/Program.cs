using Qams.BuildingBlocks.Common;
using Qams.AiAssistant.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AiAssistantStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.AiAssistant.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/assistant/prompts", (HttpRequest request, AiAssistantStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var prompts = store.ListPrompts(context.TenantId);
    return Results.Ok(ApiResponse.Ok(prompts, context.CorrelationId));
});

app.MapPost("/api/v1/assistant/requests", (AssistantRequest command, HttpRequest request, AiAssistantStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<AssistantResponse>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<AssistantResponse>(new ApiError("tenant_mismatch", "Request tenant header must match tenant."), context.CorrelationId));
    }

    var response = store.CreateResponse(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Ok(ApiResponse.Ok(response, context.CorrelationId));
});

app.Run();
