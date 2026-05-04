namespace Qams.SupplierQuality.Api;

public sealed class SupplierQualityStore
{
    private readonly object syncRoot = new();
    private readonly List<SupplierRecord> suppliers = new()
    {
        new(
            "supplier-0001",
            "tenant-life-sciences-demo",
            "Prime Components Ltd.",
            88,
            "Approved",
            DateTimeOffset.UtcNow.AddDays(-11),
            DateTimeOffset.UtcNow.AddDays(-120),
            "seed-correlation-1")
    };

    public IReadOnlyCollection<SupplierRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return suppliers
                .Where(supplier => string.IsNullOrWhiteSpace(tenantId) || supplier.TenantId == tenantId)
                .OrderByDescending(supplier => supplier.LastAuditAtUtc)
                .ToArray();
        }
    }

    public SupplierRecord? Get(string supplierId, string? tenantId)
    {
        lock (syncRoot)
        {
            return suppliers.FirstOrDefault(supplier => supplier.SupplierId == supplierId &&
                (string.IsNullOrWhiteSpace(tenantId) || supplier.TenantId == tenantId));
        }
    }

    public SupplierRecord Create(CreateSupplierRequest request, string supplierId, string correlationId)
    {
        lock (syncRoot)
        {
            var supplier = new SupplierRecord(
                supplierId,
                request.TenantId,
                request.Name,
                request.Score,
                request.Status,
                request.LastAuditAtUtc,
                DateTimeOffset.UtcNow,
                correlationId);

            suppliers.Add(supplier);
            return supplier;
        }
    }

    public SupplierRecord? UpdateScore(string supplierId, UpdateSupplierScoreRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = suppliers.FirstOrDefault(supplier => supplier.SupplierId == supplierId && supplier.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var updated = existing with
            {
                Score = request.Score,
                Status = request.Status,
                LastAuditAtUtc = request.LastAuditAtUtc,
                CorrelationId = correlationId
            };

            suppliers.Remove(existing);
            suppliers.Add(updated);
            return updated;
        }
    }
}

public sealed record SupplierRecord(
    string SupplierId,
    string TenantId,
    string Name,
    int Score,
    string Status,
    DateTimeOffset LastAuditAtUtc,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record CreateSupplierRequest(
    string TenantId,
    string Name,
    int Score,
    string Status,
    DateTimeOffset LastAuditAtUtc);

public sealed record UpdateSupplierScoreRequest(
    string TenantId,
    int Score,
    string Status,
    DateTimeOffset LastAuditAtUtc);
