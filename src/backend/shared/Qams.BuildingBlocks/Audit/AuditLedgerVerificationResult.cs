namespace Qams.BuildingBlocks.Audit;

public sealed record AuditLedgerVerificationResult(
    string TenantId,
    int EntriesChecked,
    bool IsValid,
    string Summary,
    Guid? FirstInvalidEntryId);
