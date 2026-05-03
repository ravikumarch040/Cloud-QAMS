namespace Qams.Complaints.Api;

public sealed class ComplaintsStore
{
    private readonly object syncRoot = new();
    private readonly List<ComplaintRecord> complaints = new()
    {
        new(
            "comp-0001",
            "tenant-life-sciences-demo",
            "Supplier Issue",
            "Received material with inconsistent batch labels.",
            "qa-inspector",
            DateTimeOffset.UtcNow.AddDays(-2),
            ComplaintStatus.Open,
            null,
            null,
            null,
            "seed-correlation-1")
    };

    public IReadOnlyCollection<ComplaintRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return complaints
                .Where(complaint => string.IsNullOrWhiteSpace(tenantId) || complaint.TenantId == tenantId)
                .OrderByDescending(complaint => complaint.ReportedAtUtc)
                .ToArray();
        }
    }

    public ComplaintRecord? Get(string complaintId, string? tenantId)
    {
        lock (syncRoot)
        {
            return complaints.FirstOrDefault(complaint => complaint.ComplaintId == complaintId &&
                (string.IsNullOrWhiteSpace(tenantId) || complaint.TenantId == tenantId));
        }
    }

    public ComplaintRecord Create(CreateComplaintRequest request, string complaintId, string correlationId)
    {
        lock (syncRoot)
        {
            var record = new ComplaintRecord(
                complaintId,
                request.TenantId,
                request.ComplaintType,
                request.Description,
                request.ReportedByUserId,
                request.ReportedAtUtc,
                ComplaintStatus.Open,
                null,
                null,
                null,
                correlationId);

            complaints.Add(record);
            return record;
        }
    }

    public ComplaintRecord? Resolve(string complaintId, ResolveComplaintRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = complaints.FirstOrDefault(complaint => complaint.ComplaintId == complaintId && complaint.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var resolved = existing with
            {
                Status = ComplaintStatus.Resolved,
                ResolvedByUserId = request.ResolvedByUserId,
                ResolutionNotes = request.ResolutionNotes,
                ResolvedAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId
            };

            complaints.Remove(existing);
            complaints.Add(resolved);
            return resolved;
        }
    }
}

public sealed record ComplaintRecord(
    string ComplaintId,
    string TenantId,
    string ComplaintType,
    string Description,
    string ReportedByUserId,
    DateTimeOffset ReportedAtUtc,
    ComplaintStatus Status,
    string? ResolvedByUserId,
    string? ResolutionNotes,
    DateTimeOffset? ResolvedAtUtc,
    string CorrelationId);

public sealed record CreateComplaintRequest(
    string TenantId,
    string ComplaintType,
    string Description,
    string ReportedByUserId,
    DateTimeOffset ReportedAtUtc);

public sealed record ResolveComplaintRequest(
    string TenantId,
    string ResolvedByUserId,
    string? ResolutionNotes);

public enum ComplaintStatus
{
    Open,
    Investigating,
    Resolved,
    Closed
}
