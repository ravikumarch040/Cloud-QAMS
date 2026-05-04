using Qams.BuildingBlocks.Common;
using Qams.EquipmentCalibration.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<EquipmentCalibrationStore>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.EquipmentCalibration.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/equipment-calibrations", (HttpRequest request, EquipmentCalibrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var records = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(records, context.CorrelationId));
});

app.MapGet("/api/v1/equipment-calibrations/{equipmentId}", (string equipmentId, HttpRequest request, EquipmentCalibrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var record = store.Get(equipmentId, context.TenantId);
    if (record is null)
    {
        return Results.NotFound(ApiResponse.Fail<EquipmentCalibrationRecord>(new ApiError("equipment_not_found", "Equipment calibration record not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(record, context.CorrelationId));
});

app.MapPost("/api/v1/equipment-calibrations", (CreateEquipmentCalibrationRequest command, HttpRequest request, EquipmentCalibrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<EquipmentCalibrationRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<EquipmentCalibrationRecord>(new ApiError("tenant_mismatch", "Request tenant header must match equipment tenant."), context.CorrelationId));
    }

    var created = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/equipment-calibrations/{created.EquipmentId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/equipment-calibrations/{equipmentId}/calibrate", (string equipmentId, CalibrateEquipmentRequest command, HttpRequest request, EquipmentCalibrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<EquipmentCalibrationRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<EquipmentCalibrationRecord>(new ApiError("tenant_mismatch", "Request tenant header must match equipment tenant."), context.CorrelationId));
    }

    var updated = store.Calibrate(equipmentId, command, context.TenantId, context.CorrelationId);
    if (updated is null)
    {
        return Results.NotFound(ApiResponse.Fail<EquipmentCalibrationRecord>(new ApiError("equipment_not_found", "Equipment calibration record not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(updated, context.CorrelationId));
});

app.Run();

public partial class Program { }
