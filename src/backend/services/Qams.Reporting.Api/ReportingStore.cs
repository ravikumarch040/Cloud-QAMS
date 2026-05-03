namespace Qams.Reporting.Api;

public sealed class ReportingStore
{
    private readonly object syncRoot = new();
    private readonly List<ReportDefinition> definitions = new()
    {
        new("report-001", "tenant-life-sciences-demo", "Operational KPI Summary", "Summary of key operational metrics.", new[] { "startDate", "endDate" }, "Daily"),
        new("report-002", "tenant-life-sciences-demo", "Audit Trail Export", "Export audit-ready activity logs.", Array.Empty<string>(), "Ad hoc")
    };

    public IReadOnlyCollection<ReportDefinition> ListDefinitions(string? tenantId)
    {
        lock (syncRoot)
        {
            return definitions
                .Where(def => string.IsNullOrWhiteSpace(tenantId) || def.TenantId == tenantId)
                .OrderBy(def => def.Name)
                .ToArray();
        }
    }

    public ReportResult? Generate(GenerateReportRequest request, string correlationId)
    {
        lock (syncRoot)
        {
            var definition = definitions.FirstOrDefault(def => def.ReportDefinitionId == request.ReportDefinitionId && def.TenantId == request.TenantId);
            if (definition is null)
            {
                return null;
            }

            return new ReportResult(
                Guid.NewGuid().ToString("N"),
                definition.ReportDefinitionId,
                request.TenantId,
                definition.Name,
                DateTimeOffset.UtcNow,
                request.Parameters ?? new Dictionary<string, string?>(),
                correlationId);
        }
    }
}

public sealed record ReportDefinition(
    string ReportDefinitionId,
    string TenantId,
    string Name,
    string Description,
    IReadOnlyCollection<string> Parameters,
    string Frequency);

public sealed record GenerateReportRequest(
    string TenantId,
    string ReportDefinitionId,
    Dictionary<string, string?>? Parameters);

public sealed record ReportResult(
    string ReportId,
    string ReportDefinitionId,
    string TenantId,
    string Name,
    DateTimeOffset GeneratedAtUtc,
    IReadOnlyDictionary<string, string?> Parameters,
    string CorrelationId);
