using Qams.BuildingBlocks.Audit;

namespace Qams.BuildingBlocks.Tests;

public class AuditHashTests
{
    [Fact]
    public void ComputeReturnsStableHashForSameCanonicalAuditContent()
    {
        var occurredAtUtc = new DateTimeOffset(2026, 5, 3, 12, 0, 0, TimeSpan.Zero);

        var first = AuditHash.Compute(
            "tenant-1",
            "user-1",
            "CapaClosed",
            "CapaCase",
            "capa-1",
            "3",
            occurredAtUtc,
            "correlation-1",
            "sha256:previous",
            "Effectiveness verified");

        var second = AuditHash.Compute(
            "tenant-1",
            "user-1",
            "CapaClosed",
            "CapaCase",
            "capa-1",
            "3",
            occurredAtUtc,
            "correlation-1",
            "sha256:previous",
            "Effectiveness verified");

        Assert.Equal(first, second);
        Assert.StartsWith("sha256:", first);
    }

    [Fact]
    public void ComputeChangesHashWhenCanonicalAuditContentChanges()
    {
        var occurredAtUtc = new DateTimeOffset(2026, 5, 3, 12, 0, 0, TimeSpan.Zero);

        var original = AuditHash.Compute(
            "tenant-1",
            "user-1",
            "CapaClosed",
            "CapaCase",
            "capa-1",
            "3",
            occurredAtUtc,
            "correlation-1",
            "sha256:previous",
            "Effectiveness verified");

        var tampered = AuditHash.Compute(
            "tenant-1",
            "user-1",
            "CapaClosed",
            "CapaCase",
            "capa-1",
            "4",
            occurredAtUtc,
            "correlation-1",
            "sha256:previous",
            "Effectiveness verified");

        Assert.NotEqual(original, tampered);
    }
}
