using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.EquipmentCalibration.Api",
    "Domain",
    "Scaffolded",
    ["Equipment inventory", "Calibration schedules", "Maintenance", "Out-of-tolerance impact"]));

app.Run();
