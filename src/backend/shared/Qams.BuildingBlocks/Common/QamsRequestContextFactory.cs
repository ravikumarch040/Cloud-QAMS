using Microsoft.AspNetCore.Http;

namespace Qams.BuildingBlocks.Common;

public static class QamsRequestContextFactory
{
    public static QamsRequestContext From(HttpRequest request)
    {
        var tenantId = request.Headers[QamsHeaders.TenantId].ToString();
        var correlationId = request.Headers.TryGetValue(QamsHeaders.CorrelationId, out var correlationHeader) &&
                            !string.IsNullOrWhiteSpace(correlationHeader)
            ? correlationHeader.ToString()
            : Guid.NewGuid().ToString();

        return new QamsRequestContext(
            tenantId,
            correlationId,
            request.Headers[QamsHeaders.IdempotencyKey].ToString(),
            request.Headers[QamsHeaders.ActorContext].ToString(),
            request.Headers["X-Actor-Id"].ToString() is { Length: > 0 } actorId ? actorId : "system");
    }

    public static ApiError? Validate(HttpRequest request, bool requireTenant = true, bool requireIdempotencyKey = false)
    {
        if (requireTenant && !request.Headers.ContainsKey(QamsHeaders.TenantId))
        {
            return new ApiError("missing_tenant", $"{QamsHeaders.TenantId} is required.");
        }

        if (requireIdempotencyKey && !request.Headers.ContainsKey(QamsHeaders.IdempotencyKey))
        {
            return new ApiError("missing_idempotency_key", $"{QamsHeaders.IdempotencyKey} is required for mutating requests.");
        }

        return null;
    }
}
