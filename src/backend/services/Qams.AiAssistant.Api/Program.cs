using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.AiAssistant.Api",
    "Intelligence",
    "Scaffolded",
    ["QAMS Copilot", "RCA suggestions", "CAPA drafting", "AI audit logs", "Human approval boundaries"]));

app.Run();
