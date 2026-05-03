using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.AuditInspection.Api",
    "Domain",
    "Scaffolded",
    ["Audit programs", "Inspection checklists", "Findings", "Offline evidence capture"]));

app.Run();
