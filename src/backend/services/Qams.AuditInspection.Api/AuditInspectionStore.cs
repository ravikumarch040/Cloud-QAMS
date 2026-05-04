namespace Qams.AuditInspection.Api;

public sealed class AuditInspectionStore
{
    private readonly object syncRoot = new();
    private readonly List<InspectionRecord> inspections = new()
    {
        new(
            "inspect-0001",
            "tenant-life-sciences-demo",
            "Quarterly Quality Audit",
            "Review supplier qualification and calibration records.",
            DateTimeOffset.UtcNow.AddDays(30),
            InspectionStatus.Scheduled,
            null,
            null,
            "seed-correlation-1")
    };

    public IReadOnlyCollection<InspectionRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return inspections
                .Where(item => string.IsNullOrWhiteSpace(tenantId) || item.TenantId == tenantId)
                .OrderBy(item => item.ScheduledAtUtc)
                .ToArray();
        }
    }

    public InspectionRecord? Get(string inspectionId, string? tenantId)
    {
        lock (syncRoot)
        {
            return inspections.FirstOrDefault(item => item.InspectionId == inspectionId &&
                (string.IsNullOrWhiteSpace(tenantId) || item.TenantId == tenantId));
        }
    }

    public InspectionRecord Create(CreateInspectionRequest request, string inspectionId, string correlationId)
    {
        lock (syncRoot)
        {
            var record = new InspectionRecord(
                inspectionId,
                request.TenantId,
                request.Title,
                request.Scope,
                request.ScheduledAtUtc,
                InspectionStatus.Scheduled,
                null,
                null,
                correlationId);

            inspections.Add(record);
            return record;
        }
    }

    public InspectionRecord? Complete(string inspectionId, CompleteInspectionRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = inspections.FirstOrDefault(item => item.InspectionId == inspectionId && item.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var completed = existing with
            {
                Status = InspectionStatus.Completed,
                Findings = request.Findings,
                CompletedAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId
            };

            inspections.Remove(existing);
            inspections.Add(completed);
            return completed;
        }
    }
}

public sealed record InspectionRecord(
    string InspectionId,
    string TenantId,
    string Title,
    string Scope,
    DateTimeOffset ScheduledAtUtc,
    InspectionStatus Status,
    string? Findings,
    DateTimeOffset? CompletedAtUtc,
    string CorrelationId);

public sealed record CreateInspectionRequest(
    string TenantId,
    string Title,
    string Scope,
    DateTimeOffset ScheduledAtUtc);

public sealed record CompleteInspectionRequest(
    string TenantId,
    string Findings);

public enum InspectionStatus
{
    Scheduled,
    InProgress,
    Completed,
    Closed
}
