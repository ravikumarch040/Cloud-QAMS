using Qams.BuildingBlocks.Audit;

namespace Qams.AuditLedger.Api;

public sealed class AuditLedgerStore
{
    private readonly object syncRoot = new();
    private readonly List<AuditLedgerEntry> entries = [];

    public AuditLedgerStore()
    {
        Append(new AppendAuditLedgerEntryRequest(
            "tenant-life-sciences-demo",
            "system",
            "TenantProvisioned",
            "Tenant",
            "tenant-life-sciences-demo",
            "1",
            "Starter ledger entry"),
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.NewGuid().ToString());
    }

    public IReadOnlyCollection<AuditLedgerEntry> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return entries
                .Where(entry => string.IsNullOrWhiteSpace(tenantId) || entry.TenantId == tenantId)
                .OrderBy(entry => entry.OccurredAtUtc)
                .ToArray();
        }
    }

    public AuditLedgerEntry Append(AppendAuditLedgerEntryRequest request, Guid auditEntryId, string correlationId)
    {
        lock (syncRoot)
        {
            var occurredAtUtc = DateTimeOffset.UtcNow;
            var previousHash = entries
                .Where(entry => entry.TenantId == request.TenantId)
                .OrderByDescending(entry => entry.OccurredAtUtc)
                .Select(entry => entry.RecordHash)
                .FirstOrDefault();

            var hash = AuditHash.Compute(
                request.TenantId,
                request.ActorId,
                request.Action,
                request.RecordType,
                request.RecordId,
                request.RecordVersion,
                occurredAtUtc,
                correlationId,
                previousHash,
                request.Reason);

            var entry = new AuditLedgerEntry(
                auditEntryId,
                request.TenantId,
                request.ActorId,
                request.Action,
                request.RecordType,
                request.RecordId,
                request.RecordVersion,
                occurredAtUtc,
                correlationId,
                hash,
                previousHash,
                request.Reason);

            entries.Add(entry);
            return entry;
        }
    }

    public AuditLedgerVerificationResult Verify(string tenantId)
    {
        lock (syncRoot)
        {
            var tenantEntries = entries
                .Where(entry => entry.TenantId == tenantId)
                .OrderBy(entry => entry.OccurredAtUtc)
                .ToArray();

            string? previousHash = null;
            foreach (var entry in tenantEntries)
            {
                if (entry.PreviousHash != previousHash)
                {
                    return new AuditLedgerVerificationResult(
                        tenantId,
                        tenantEntries.Length,
                        false,
                        "Audit ledger hash chain is broken: previous hash does not match expected value.",
                        entry.AuditEntryId);
                }

                var expectedHash = AuditHash.Compute(
                    entry.TenantId,
                    entry.ActorId,
                    entry.Action,
                    entry.RecordType,
                    entry.RecordId,
                    entry.RecordVersion,
                    entry.OccurredAtUtc,
                    entry.CorrelationId,
                    entry.PreviousHash,
                    entry.Reason);

                if (!string.Equals(entry.RecordHash, expectedHash, StringComparison.Ordinal))
                {
                    return new AuditLedgerVerificationResult(
                        tenantId,
                        tenantEntries.Length,
                        false,
                        "Audit ledger hash chain is broken: entry hash does not match canonical content.",
                        entry.AuditEntryId);
                }

                previousHash = entry.RecordHash;
            }

            return new AuditLedgerVerificationResult(
                tenantId,
                tenantEntries.Length,
                true,
                "Audit ledger hash chain verification passed.",
                null);
        }
    }
}

public sealed record AppendAuditLedgerEntryRequest(
    string TenantId,
    string ActorId,
    string Action,
    string RecordType,
    string RecordId,
    string RecordVersion,
    string? Reason);
