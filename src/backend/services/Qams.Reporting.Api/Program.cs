using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Reporting.Api",
    "Platform",
    "Scaffolded",
    ["Operational reports", "Dashboards", "Audit-ready exports", "Scheduled reporting"]));

app.Run();
