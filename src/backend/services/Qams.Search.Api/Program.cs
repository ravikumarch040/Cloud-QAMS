using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Search.Api",
    "Intelligence",
    "Scaffolded",
    ["Keyword search", "Semantic search", "Security trimming", "Vector indexes"]));

app.Run();
