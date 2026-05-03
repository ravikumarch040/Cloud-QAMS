using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.SupplierQuality.Api",
    "Domain",
    "Scaffolded",
    ["Supplier qualification", "SCAR", "Supplier audits", "Scorecards", "Certificates"]));

app.Run();
