using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.DocumentControl.Api",
    "Domain",
    "Scaffolded",
    ["SOP lifecycle", "Document versions", "Approvals", "Effective dates", "Controlled copies"]));

app.Run();
