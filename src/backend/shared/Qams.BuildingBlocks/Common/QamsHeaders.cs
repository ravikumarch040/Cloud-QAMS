namespace Qams.BuildingBlocks.Common;

public static class QamsHeaders
{
    public const string TenantId = "X-Tenant-Id";
    public const string CorrelationId = "X-Correlation-Id";
    public const string IdempotencyKey = "Idempotency-Key";
    public const string ActorContext = "X-Actor-Context";
}
