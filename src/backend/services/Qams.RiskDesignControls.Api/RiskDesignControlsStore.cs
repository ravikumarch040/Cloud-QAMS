namespace Qams.RiskDesignControls.Api;

public sealed class RiskDesignControlsStore
{
    private readonly object syncRoot = new();
    private readonly List<RiskAssessmentRecord> assessments = new()
    {
        new(
            "risk-0001",
            "tenant-life-sciences-demo",
            "Device Release Risk Assessment",
            "Assess risk for device release controls.",
            "High",
            RiskAssessmentStatus.Open,
            DateTimeOffset.UtcNow.AddDays(-7),
            "seed-correlation-1")
    };

    public IReadOnlyCollection<RiskAssessmentRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return assessments
                .Where(assessment => string.IsNullOrWhiteSpace(tenantId) || assessment.TenantId == tenantId)
                .OrderByDescending(assessment => assessment.CreatedAtUtc)
                .ToArray();
        }
    }

    public RiskAssessmentRecord Create(CreateRiskAssessmentRequest request, string riskAssessmentId, string correlationId)
    {
        lock (syncRoot)
        {
            var assessment = new RiskAssessmentRecord(
                riskAssessmentId,
                request.TenantId,
                request.Title,
                request.Description,
                request.RiskLevel,
                RiskAssessmentStatus.Open,
                DateTimeOffset.UtcNow,
                correlationId);

            assessments.Add(assessment);
            return assessment;
        }
    }
}

public sealed record RiskAssessmentRecord(
    string RiskAssessmentId,
    string TenantId,
    string Title,
    string Description,
    string RiskLevel,
    RiskAssessmentStatus Status,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record CreateRiskAssessmentRequest(
    string TenantId,
    string Title,
    string Description,
    string RiskLevel);

public enum RiskAssessmentStatus
{
    Open,
    Mitigated,
    Closed
}
