using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Rules.Api",
    "Platform",
    "Scaffolded",
    ["Business rules", "Automation triggers", "SLA escalations", "Impact assessment rules"]));

app.Run();
