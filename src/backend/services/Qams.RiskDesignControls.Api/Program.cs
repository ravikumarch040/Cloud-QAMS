using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.RiskDesignControls.Api",
    "Domain",
    "Scaffolded",
    ["Risk files", "Hazards", "Design controls", "Traceability", "Risk review"]));

app.Run();
