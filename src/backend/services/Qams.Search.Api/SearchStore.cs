namespace Qams.Search.Api;

public sealed class SearchStore
{
    private readonly object syncRoot = new();
    private readonly List<SearchIndexEntry> index = new()
    {
        new("doc-0001", "tenant-life-sciences-demo", "Supplier Quality Scorecard", "Quality metrics and supplier performance review."),
        new("doc-0002", "tenant-life-sciences-demo", "Calibration Audit Summary", "Audit findings for equipment calibration records."),
        new("doc-0003", "tenant-life-sciences-demo", "Management Review Notes", "KPIs and action items for management review.")
    };

    public IReadOnlyCollection<SearchResult> Search(string query, string? tenantId)
    {
        lock (syncRoot)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<SearchResult>();
            }

            var trimmedQuery = query.Trim();
            return index
                .Where(entry => (string.IsNullOrWhiteSpace(tenantId) || entry.TenantId == tenantId) &&
                    (entry.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) || entry.Snippet.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase)))
                .Select(entry => new SearchResult(entry.DocumentId, entry.Title, entry.Snippet, 1.0m))
                .ToArray();
        }
    }
}

public sealed record SearchIndexEntry(
    string DocumentId,
    string TenantId,
    string Title,
    string Snippet);

public sealed record SearchResult(
    string DocumentId,
    string Title,
    string Snippet,
    decimal RelevanceScore);
