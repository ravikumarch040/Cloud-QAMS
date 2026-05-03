namespace Qams.BuildingBlocks.Compliance;

public sealed record ESignatureRecord(
    Guid SignatureId,
    string TenantId,
    string RecordType,
    string RecordId,
    string RecordVersion,
    string SignedMeaning,
    string Reason,
    string SignerUserId,
    string SignerDisplayName,
    string ReauthMethod,
    string ReauthReference,
    DateTimeOffset SignedAtUtc,
    string RecordHash,
    string? PreviousAuditHash,
    Guid AuditEntryId);
