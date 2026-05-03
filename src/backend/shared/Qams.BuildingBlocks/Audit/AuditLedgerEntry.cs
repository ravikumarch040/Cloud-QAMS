namespace Qams.BuildingBlocks.Audit;

public sealed record AuditLedgerEntry(
    Guid AuditEntryId,
    string TenantId,
    string ActorId,
    string Action,
    string RecordType,
    string RecordId,
    string RecordVersion,
    DateTimeOffset OccurredAtUtc,
    string CorrelationId,
    string RecordHash,
    string? PreviousHash,
    string? Reason);
