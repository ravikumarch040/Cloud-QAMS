using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Complaints.Api",
    "Domain",
    "Scaffolded",
    ["Complaint intake", "Triage", "Investigation", "Reportability assessment"]));

app.Run();
