namespace Qams.ValidationEvidence.Api;

public sealed class ValidationEvidenceStore
{
    private readonly object syncRoot = new();
    private readonly List<ValidationEvidencePack> evidencePacks = new()
    {
        new(
            "pack-demo-001",
            "tenant-life-sciences-demo",
            "Validation Pack for Device Release",
            "Comprehensive traceability package for release validation.",
            ValidationEvidenceStatus.Pending,
            new[] { "REQ-001", "REQ-002", "REQ-003" },
            new[]
            {
                new ValidationEvidenceItem("item-1", "Device Master Record", "Draft device master record package."),
                new ValidationEvidenceItem("item-2", "Test Report", "Final verification test report."),
            },
            DateTimeOffset.UtcNow.AddHours(-2),
            null,
            null,
            "seed-correlation-1")
    };

    public IReadOnlyCollection<ValidationEvidencePack> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return evidencePacks
                .Where(pack => string.IsNullOrWhiteSpace(tenantId) || pack.TenantId == tenantId)
                .OrderByDescending(pack => pack.CreatedAtUtc)
                .ToArray();
        }
    }

    public ValidationEvidencePack? Get(string evidencePackId, string? tenantId)
    {
        lock (syncRoot)
        {
            return evidencePacks.FirstOrDefault(pack => pack.EvidencePackId == evidencePackId &&
                (string.IsNullOrWhiteSpace(tenantId) || pack.TenantId == tenantId));
        }
    }

    public ValidationEvidencePack Create(CreateValidationEvidenceRequest request, string evidencePackId, string correlationId)
    {
        lock (syncRoot)
        {
            var evidencePack = new ValidationEvidencePack(
                evidencePackId,
                request.TenantId,
                request.Title,
                request.Description,
                ValidationEvidenceStatus.Pending,
                request.RelatedRequirementIds?.ToArray() ?? Array.Empty<string>(),
                request.EvidenceItems?.ToArray() ?? Array.Empty<ValidationEvidenceItem>(),
                DateTimeOffset.UtcNow,
                null,
                null,
                correlationId);

            evidencePacks.Add(evidencePack);
            return evidencePack;
        }
    }

    public ValidationEvidencePack? Approve(string evidencePackId, ApproveValidationEvidenceRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = evidencePacks.FirstOrDefault(pack => pack.EvidencePackId == evidencePackId && pack.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var approved = existing with
            {
                Status = ValidationEvidenceStatus.Approved,
                ApprovedAtUtc = DateTimeOffset.UtcNow,
                ApprovedByUserId = request.ApprovedByUserId,
                CorrelationId = correlationId
            };

            evidencePacks.Remove(existing);
            evidencePacks.Add(approved);
            return approved;
        }
    }
}

public sealed record ValidationEvidencePack(
    string EvidencePackId,
    string TenantId,
    string Title,
    string Description,
    ValidationEvidenceStatus Status,
    IReadOnlyCollection<string> RelatedRequirementIds,
    IReadOnlyCollection<ValidationEvidenceItem> EvidenceItems,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ApprovedAtUtc,
    string? ApprovedByUserId,
    string CorrelationId);

public sealed record ValidationEvidenceItem(
    string ItemId,
    string Name,
    string Description);

public sealed record CreateValidationEvidenceRequest(
    string TenantId,
    string Title,
    string Description,
    IReadOnlyCollection<string>? RelatedRequirementIds,
    IReadOnlyCollection<ValidationEvidenceItem>? EvidenceItems);

public sealed record ApproveValidationEvidenceRequest(
    string TenantId,
    string ApprovedByUserId,
    string? ApprovalComments);

public enum ValidationEvidenceStatus
{
    Pending,
    Approved,
    Rejected
}
