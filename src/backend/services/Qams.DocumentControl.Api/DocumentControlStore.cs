namespace Qams.DocumentControl.Api;

public sealed class DocumentControlStore
{
    private readonly object syncRoot = new();
    private readonly List<DocumentControlRecord> documents = new()
    {
        new(
            "doc-001",
            "tenant-life-sciences-demo",
            "Controlled Document: Device Release Procedure",
            "Procedure for signed release of medical device software.",
            DocumentStatus.Draft,
            new[]
            {
                new DocumentVersion("v1.0", "Initial draft", "5d41402abc4b2a76b9719d911017c592"),
            },
            DateTimeOffset.UtcNow.AddDays(-10),
            null,
            null,
            "seed-correlation-1")
    };

    public IReadOnlyCollection<DocumentControlRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return documents
                .Where(doc => string.IsNullOrWhiteSpace(tenantId) || doc.TenantId == tenantId)
                .OrderByDescending(doc => doc.CreatedAtUtc)
                .ToArray();
        }
    }

    public DocumentControlRecord? Get(string documentId, string? tenantId)
    {
        lock (syncRoot)
        {
            return documents.FirstOrDefault(doc => doc.DocumentId == documentId &&
                (string.IsNullOrWhiteSpace(tenantId) || doc.TenantId == tenantId));
        }
    }

    public DocumentControlRecord Create(CreateDocumentRequest request, string documentId, string correlationId)
    {
        lock (syncRoot)
        {
            var record = new DocumentControlRecord(
                documentId,
                request.TenantId,
                request.Title,
                request.Description,
                DocumentStatus.Draft,
                request.InitialVersions?.ToArray() ?? Array.Empty<DocumentVersion>(),
                DateTimeOffset.UtcNow,
                null,
                null,
                correlationId);

            documents.Add(record);
            return record;
        }
    }

    public DocumentControlRecord? AddVersion(string documentId, AddDocumentVersionRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = documents.FirstOrDefault(doc => doc.DocumentId == documentId && doc.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var updated = existing with
            {
                Versions = existing.Versions.Concat(new[] { new DocumentVersion(request.VersionLabel, request.ChangeDescription, request.ContentHash) }).ToArray(),
                CorrelationId = correlationId
            };

            documents.Remove(existing);
            documents.Add(updated);
            return updated;
        }
    }

    public DocumentControlRecord? Approve(string documentId, ApproveDocumentRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = documents.FirstOrDefault(doc => doc.DocumentId == documentId && doc.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var approved = existing with
            {
                Status = DocumentStatus.Approved,
                ApprovedAtUtc = DateTimeOffset.UtcNow,
                ApprovedByUserId = request.ApprovedByUserId,
                CorrelationId = correlationId
            };

            documents.Remove(existing);
            documents.Add(approved);
            return approved;
        }
    }
}

public sealed record DocumentControlRecord(
    string DocumentId,
    string TenantId,
    string Title,
    string Description,
    DocumentStatus Status,
    IReadOnlyCollection<DocumentVersion> Versions,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ApprovedAtUtc,
    string? ApprovedByUserId,
    string CorrelationId);

public sealed record DocumentVersion(
    string VersionLabel,
    string ChangeDescription,
    string ContentHash);

public sealed record CreateDocumentRequest(
    string TenantId,
    string Title,
    string Description,
    IReadOnlyCollection<DocumentVersion>? InitialVersions);

public sealed record AddDocumentVersionRequest(
    string TenantId,
    string VersionLabel,
    string ChangeDescription,
    string ContentHash);

public sealed record ApproveDocumentRequest(
    string TenantId,
    string ApprovedByUserId,
    string? ApprovalComments);

public enum DocumentStatus
{
    Draft,
    Released,
    Approved,
    Superseded
}
