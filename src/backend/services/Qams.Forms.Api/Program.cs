using Qams.BuildingBlocks.ServiceInfo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapQamsServiceInfo(new QamsServiceDescriptor(
    "Qams.Forms.Api",
    "Platform",
    "Scaffolded",
    ["Versioned forms", "Field validation", "Controlled lists", "Form impact assessment"]));

app.Run();
