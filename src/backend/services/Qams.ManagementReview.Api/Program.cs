using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.ManagementReview.Api",
    "Domain",
    "Scaffolded",
    ["Management review packs", "Quality KPIs", "Review actions", "Leadership evidence"]));

app.Run();
