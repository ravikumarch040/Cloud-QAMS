namespace Qams.ManagementReview.Api;

public sealed class ManagementReviewStore
{
    private readonly object syncRoot = new();
    private readonly List<ReviewPackRecord> reviewPacks = new()
    {
        new(
            "review-0001",
            "tenant-life-sciences-demo",
            "Q2 Management Review",
            "Quarterly review of quality KPIs and action plans.",
            DateTimeOffset.UtcNow.AddDays(-7),
            ReviewPackStatus.Pending,
            Array.Empty<string>(),
            DateTimeOffset.UtcNow.AddDays(-9),
            "seed-correlation-1")
    };

    public IReadOnlyCollection<ReviewPackRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return reviewPacks
                .Where(pack => string.IsNullOrWhiteSpace(tenantId) || pack.TenantId == tenantId)
                .OrderByDescending(pack => pack.ReviewDateUtc)
                .ToArray();
        }
    }

    public ReviewPackRecord? Get(string reviewPackId, string? tenantId)
    {
        lock (syncRoot)
        {
            return reviewPacks.FirstOrDefault(pack => pack.ReviewPackId == reviewPackId &&
                (string.IsNullOrWhiteSpace(tenantId) || pack.TenantId == tenantId));
        }
    }

    public ReviewPackRecord Create(CreateReviewPackRequest request, string reviewPackId, string correlationId)
    {
        lock (syncRoot)
        {
            var pack = new ReviewPackRecord(
                reviewPackId,
                request.TenantId,
                request.Title,
                request.Summary,
                request.ReviewDateUtc,
                ReviewPackStatus.Pending,
                request.ActionItems?.ToArray() ?? Array.Empty<string>(),
                DateTimeOffset.UtcNow,
                correlationId);

            reviewPacks.Add(pack);
            return pack;
        }
    }
}

public sealed record ReviewPackRecord(
    string ReviewPackId,
    string TenantId,
    string Title,
    string Summary,
    DateTimeOffset ReviewDateUtc,
    ReviewPackStatus Status,
    IReadOnlyCollection<string> ActionItems,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record CreateReviewPackRequest(
    string TenantId,
    string Title,
    string Summary,
    DateTimeOffset ReviewDateUtc,
    IReadOnlyCollection<string>? ActionItems);

public enum ReviewPackStatus
{
    Pending,
    Completed,
    Archived
}
