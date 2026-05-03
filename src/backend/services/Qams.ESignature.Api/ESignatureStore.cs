using Qams.BuildingBlocks.Compliance;

namespace Qams.ESignature.Api;

public sealed class ESignatureStore
{
    private readonly object syncRoot = new();
    private readonly List<ESignatureRecord> signatures = [];

    public IReadOnlyCollection<ESignatureRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return signatures
                .Where(s => string.IsNullOrWhiteSpace(tenantId) || s.TenantId == tenantId)
                .OrderByDescending(s => s.SignedAtUtc)
                .ToArray();
        }
    }

    public ESignatureRecord Create(CreateESignatureRequest request, Guid signatureId, string correlationId)
    {
        lock (syncRoot)
        {
            var signedAtUtc = DateTimeOffset.UtcNow;

            // TODO: Create audit ledger entry and get auditEntryId
            var auditEntryId = Guid.NewGuid(); // Placeholder

            var signature = new ESignatureRecord(
                signatureId,
                request.TenantId,
                request.RecordType,
                request.RecordId,
                request.RecordVersion,
                request.SignedMeaning,
                request.Reason,
                request.SignerUserId,
                request.SignerDisplayName,
                request.ReauthMethod,
                request.ReauthReference,
                signedAtUtc,
                request.RecordHash,
                request.PreviousAuditHash,
                auditEntryId);

            signatures.Add(signature);
            return signature;
        }
    }
}

public sealed record CreateESignatureRequest(
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
    string RecordHash,
    string? PreviousAuditHash);