using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.ChangeControl.Api",
    "Domain",
    "Scaffolded",
    ["Change requests", "Impact assessment", "Implementation tasks", "Change approvals"]));

app.Run();
