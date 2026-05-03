using System.Security.Cryptography;
using System.Text;

namespace Qams.BuildingBlocks.Audit;

public static class AuditHash
{
    public static string Compute(
        string tenantId,
        string actorId,
        string action,
        string recordType,
        string recordId,
        string recordVersion,
        DateTimeOffset occurredAtUtc,
        string correlationId,
        string? previousHash,
        string? reason)
    {
        var canonical = string.Join(
            "|",
            tenantId,
            actorId,
            action,
            recordType,
            recordId,
            recordVersion,
            occurredAtUtc.ToUniversalTime().ToString("O"),
            correlationId,
            previousHash ?? string.Empty,
            reason ?? string.Empty);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return $"sha256:{Convert.ToHexString(bytes).ToLowerInvariant()}";
    }
}
