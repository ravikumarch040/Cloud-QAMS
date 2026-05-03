using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Notification.Api",
    "Platform",
    "Scaffolded",
    ["In-app notifications", "Email notifications", "Escalations", "Workflow reminders"]));

app.Run();
