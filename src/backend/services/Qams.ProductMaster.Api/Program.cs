using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.ProductMaster.Api",
    "Domain",
    "Scaffolded",
    ["Products", "Devices", "Lots", "Reference data", "Affected product tracing"]));

app.Run();
