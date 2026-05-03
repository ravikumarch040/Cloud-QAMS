namespace Qams.EquipmentCalibration.Api;

public sealed class EquipmentCalibrationStore
{
    private readonly object syncRoot = new();
    private readonly List<EquipmentCalibrationRecord> records = new()
    {
        new(
            "eq-1001",
            "tenant-life-sciences-demo",
            "Thermocycler Model X",
            "Calibration required every 90 days.",
            DateTimeOffset.UtcNow.AddDays(-45),
            DateTimeOffset.UtcNow.AddDays(45),
            CalibrationStatus.Due,
            "seed-correlation-1")
    };

    public IReadOnlyCollection<EquipmentCalibrationRecord> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return records
                .Where(record => string.IsNullOrWhiteSpace(tenantId) || record.TenantId == tenantId)
                .OrderByDescending(record => record.NextCalibrationDueAtUtc)
                .ToArray();
        }
    }

    public EquipmentCalibrationRecord? Get(string equipmentId, string? tenantId)
    {
        lock (syncRoot)
        {
            return records.FirstOrDefault(record => record.EquipmentId == equipmentId &&
                (string.IsNullOrWhiteSpace(tenantId) || record.TenantId == tenantId));
        }
    }

    public EquipmentCalibrationRecord Create(CreateEquipmentCalibrationRequest request, string equipmentId, string correlationId)
    {
        lock (syncRoot)
        {
            var record = new EquipmentCalibrationRecord(
                equipmentId,
                request.TenantId,
                request.EquipmentName,
                request.Description,
                request.LastCalibrationAtUtc,
                request.NextCalibrationDueAtUtc,
                CalibrationStatus.Scheduled,
                correlationId);

            records.Add(record);
            return record;
        }
    }

    public EquipmentCalibrationRecord? Calibrate(string equipmentId, CalibrateEquipmentRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var existing = records.FirstOrDefault(record => record.EquipmentId == equipmentId && record.TenantId == tenantId);
            if (existing is null)
            {
                return null;
            }

            var updated = existing with
            {
                LastCalibrationAtUtc = request.CalibrationAtUtc,
                NextCalibrationDueAtUtc = request.CalibrationAtUtc.AddDays(request.IntervalDays),
                Status = request.OutOfTolerance ? CalibrationStatus.OutOfTolerance : CalibrationStatus.Scheduled,
                CorrelationId = correlationId
            };

            records.Remove(existing);
            records.Add(updated);
            return updated;
        }
    }
}

public sealed record EquipmentCalibrationRecord(
    string EquipmentId,
    string TenantId,
    string EquipmentName,
    string Description,
    DateTimeOffset LastCalibrationAtUtc,
    DateTimeOffset NextCalibrationDueAtUtc,
    CalibrationStatus Status,
    string CorrelationId);

public sealed record CreateEquipmentCalibrationRequest(
    string TenantId,
    string EquipmentName,
    string Description,
    DateTimeOffset LastCalibrationAtUtc,
    DateTimeOffset NextCalibrationDueAtUtc);

public sealed record CalibrateEquipmentRequest(
    string TenantId,
    DateTimeOffset CalibrationAtUtc,
    int IntervalDays,
    bool OutOfTolerance);

public enum CalibrationStatus
{
    Scheduled,
    Due,
    OutOfTolerance,
    Completed
}
