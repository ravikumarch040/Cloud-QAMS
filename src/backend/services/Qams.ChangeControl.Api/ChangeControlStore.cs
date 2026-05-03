namespace Qams.ChangeControl.Api;

public sealed class ChangeControlStore
{
    private readonly object syncRoot = new();
    private readonly List<ChangeRequestRecord> changeRequests = new()
    {
        new(
            "cc-0001",
            "tenant-life-sciences-demo",
            "Update device release protocol",
            "Revise release verification steps to include new software validation checks.",
            ChangeRequestStatus.Pending,
            "Impact assessment pending",
            DateTimeOffset.UtcNow.AddDays(-3),
            null,
            null,
            "seed-correlation-1")
    };

    public IReadOnlyCollection<ChangeRequestRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return changeRequests
                .Where(request => string.IsNullOrWhiteSpace(tenantId) || request.TenantId == tenantId)
                .OrderByDescending(request => request.CreatedAtUtc)
                .ToArray();
        }
    }

    public ChangeRequestRecord? Get(string changeRequestId, string? tenantId)
    {
        lock (syncRoot)
        {
            return changeRequests.FirstOrDefault(request => request.ChangeRequestId == changeRequestId &&
                (string.IsNullOrWhiteSpace(tenantId) || request.TenantId == tenantId));
        }
    }

    public ChangeRequestRecord Create(CreateChangeRequestRequest request, string changeRequestId, string correlationId)
    {
        lock (syncRoot)
        {
            var record = new ChangeRequestRecord(
                changeRequestId,
                request.TenantId,
                request.Title,
                request.Description,
                ChangeRequestStatus.Pending,
                request.ImpactAnalysis,
                DateTimeOffset.UtcNow,
                null,
                null,
                correlationId);

            changeRequests.Add(record);
            return record;
        }
    }

    public ChangeRequestRecord? Approve(string changeRequestId, ApproveChangeRequestRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = changeRequests.FirstOrDefault(item => item.ChangeRequestId == changeRequestId && item.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var approved = existing with
            {
                Status = ChangeRequestStatus.Approved,
                ApprovedByUserId = request.ApprovedByUserId,
                ApprovalComments = request.ApprovalComments,
                ApprovedAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId
            };

            changeRequests.Remove(existing);
            changeRequests.Add(approved);
            return approved;
        }
    }
}

public sealed record ChangeRequestRecord(
    string ChangeRequestId,
    string TenantId,
    string Title,
    string Description,
    ChangeRequestStatus Status,
    string ImpactAnalysis,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ApprovedAtUtc,
    string? ApprovedByUserId,
    string CorrelationId,
    string? ApprovalComments = null);

public sealed record CreateChangeRequestRequest(
    string TenantId,
    string Title,
    string Description,
    string ImpactAnalysis);

public sealed record ApproveChangeRequestRequest(
    string TenantId,
    string ApprovedByUserId,
    string? ApprovalComments);

public enum ChangeRequestStatus
{
    Pending,
    Approved,
    Rejected
}
