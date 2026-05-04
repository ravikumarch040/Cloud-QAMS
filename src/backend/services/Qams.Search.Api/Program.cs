using Qams.BuildingBlocks.Common;
using Qams.Search.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SearchStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Search.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/search", (HttpRequest request, SearchStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var query = request.Query["query"].ToString();
    var results = store.Search(query, context.TenantId);
    return Results.Ok(ApiResponse.Ok(results, context.CorrelationId));
});

app.Run();
