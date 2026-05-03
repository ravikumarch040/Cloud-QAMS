using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Qams.BuildingBlocks.Common;

namespace Qams.BuildingBlocks.ServiceInfo;

public static class QamsServiceInfoEndpoints
{
    public static void MapQamsServiceInfo(this WebApplication app, QamsServiceDescriptor descriptor)
    {
        app.MapGet("/health", () => Results.Ok(new
        {
            service = descriptor.ServiceName,
            status = "Healthy",
            utc = DateTimeOffset.UtcNow
        }));

        app.MapGet("/api/v1/service-info", (HttpRequest request) =>
        {
            var context = QamsRequestContextFactory.From(request);
            return Results.Ok(ApiResponse.Ok(descriptor, context.CorrelationId));
        });
    }
}
