using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Integration.Api",
    "Platform",
    "Scaffolded",
    ["ERP integration", "MES/LIMS integration", "PLM integration", "Webhooks", "Batch imports"]));

app.Run();
