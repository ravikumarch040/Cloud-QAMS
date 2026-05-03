namespace Qams.BuildingBlocks.Common;

public sealed record QamsRequestContext(
    string TenantId,
    string CorrelationId,
    string? IdempotencyKey,
    string? ActorContext,
    string ActorId);
