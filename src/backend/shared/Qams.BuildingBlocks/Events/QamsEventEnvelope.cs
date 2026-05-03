namespace Qams.BuildingBlocks.Events;

public sealed record QamsEventEnvelope<TPayload>(
    Guid EventId,
    string EventType,
    string SchemaVersion,
    string TenantId,
    string ActorId,
    DateTimeOffset OccurredAtUtc,
    string CorrelationId,
    string? CausationId,
    string DataClassification,
    TPayload Payload);
