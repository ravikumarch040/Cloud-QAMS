namespace Qams.ProductMaster.Api;

public sealed class ProductMasterStore
{
    private readonly object syncRoot = new();
    private readonly List<ProductRecord> products = new()
    {
        new(
            "product-0001",
            "tenant-life-sciences-demo",
            "MD-1001",
            "Medical Device A",
            "Primary device for quality assurance testing.",
            DateTimeOffset.UtcNow.AddDays(-60),
            "seed-correlation-1")
    };

    public IReadOnlyCollection<ProductRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return products
                .Where(product => string.IsNullOrWhiteSpace(tenantId) || product.TenantId == tenantId)
                .OrderBy(product => product.ProductCode)
                .ToArray();
        }
    }

    public ProductRecord? Get(string productId, string? tenantId)
    {
        lock (syncRoot)
        {
            return products.FirstOrDefault(product => product.ProductId == productId &&
                (string.IsNullOrWhiteSpace(tenantId) || product.TenantId == tenantId));
        }
    }

    public ProductRecord Create(CreateProductRequest request, string productId, string correlationId)
    {
        lock (syncRoot)
        {
            var product = new ProductRecord(
                productId,
                request.TenantId,
                request.ProductCode,
                request.Name,
                request.Description,
                DateTimeOffset.UtcNow,
                correlationId);

            products.Add(product);
            return product;
        }
    }
}

public sealed record ProductRecord(
    string ProductId,
    string TenantId,
    string ProductCode,
    string Name,
    string Description,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record CreateProductRequest(
    string TenantId,
    string ProductCode,
    string Name,
    string Description);
