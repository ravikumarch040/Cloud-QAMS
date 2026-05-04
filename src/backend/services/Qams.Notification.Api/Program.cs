using Qams.BuildingBlocks.Common;
using Qams.Notification.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<NotificationStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Notification.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/notifications", (HttpRequest request, NotificationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var notifications = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(notifications, context.CorrelationId));
});

app.MapPost("/api/v1/notifications", (CreateNotificationRequest command, HttpRequest request, NotificationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<NotificationEntry>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<NotificationEntry>(
            new ApiError("tenant_mismatch", "Request tenant header must match notification tenant."),
            context.CorrelationId));
    }

    var notification = store.CreateNotification(command, Guid.NewGuid(), context.CorrelationId);
    return Results.Created($"/api/v1/notifications/{notification.NotificationId}", ApiResponse.Ok(notification, context.CorrelationId));
});

app.MapPost("/api/v1/notifications/email", (CreateEmailNotificationRequest command, HttpRequest request, NotificationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<NotificationEntry>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<NotificationEntry>(
            new ApiError("tenant_mismatch", "Request tenant header must match email notification tenant."),
            context.CorrelationId));
    }

    var notification = store.CreateEmailNotification(command, Guid.NewGuid(), context.CorrelationId);
    return Results.Created($"/api/v1/notifications/{notification.NotificationId}", ApiResponse.Ok(notification, context.CorrelationId));
});

app.MapPost("/api/v1/notifications/{notificationId}/acknowledge", (string notificationId, HttpRequest request, NotificationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<NotificationEntry>(headerError, context.CorrelationId));
    }

    var acknowledged = store.Acknowledge(notificationId, context.TenantId, context.CorrelationId);
    if (acknowledged is null)
    {
        return Results.NotFound(ApiResponse.Fail<NotificationEntry>(new ApiError("notification_not_found", "Notification not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(acknowledged, context.CorrelationId));
});

app.Run();

public partial class Program { }
