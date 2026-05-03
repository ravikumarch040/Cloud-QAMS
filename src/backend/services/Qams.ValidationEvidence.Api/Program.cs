using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.ValidationEvidence.Api",
    "Platform",
    "Scaffolded",
    ["Requirements traceability", "Control mapping", "Test evidence", "Release validation packs"]));

app.Run();
