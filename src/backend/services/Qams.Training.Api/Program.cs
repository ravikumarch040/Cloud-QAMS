using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Training.Api",
    "Domain",
    "Scaffolded",
    ["Training assignments", "Curricula", "Retraining triggers", "Competency evidence"]));

app.Run();
