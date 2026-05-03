namespace Qams.Notification.Api;

public sealed class NotificationStore
{
    private readonly object syncRoot = new();
    private readonly List<NotificationEntry> notifications = [];

    public IReadOnlyCollection<NotificationEntry> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return notifications
                .Where(notification => string.IsNullOrWhiteSpace(tenantId) || notification.TenantId == tenantId)
                .OrderByDescending(notification => notification.CreatedAtUtc)
                .ToArray();
        }
    }

    public NotificationEntry CreateNotification(CreateNotificationRequest request, Guid notificationId, string correlationId)
    {
        lock (syncRoot)
        {
            var entry = new NotificationEntry(
                notificationId,
                request.TenantId,
                request.RecipientUserId,
                request.Subject,
                request.Message,
                NotificationType.InApp,
                false,
                DateTimeOffset.UtcNow,
                correlationId);

            notifications.Add(entry);
            return entry;
        }
    }

    public NotificationEntry CreateEmailNotification(CreateEmailNotificationRequest request, Guid notificationId, string correlationId)
    {
        lock (syncRoot)
        {
            var entry = new NotificationEntry(
                notificationId,
                request.TenantId,
                request.RecipientUserId,
                request.Subject,
                request.Message,
                NotificationType.Email,
                false,
                DateTimeOffset.UtcNow,
                correlationId,
                request.EmailAddress);

            notifications.Add(entry);
            return entry;
        }
    }

    public NotificationEntry? Acknowledge(string notificationId, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var entry = notifications.FirstOrDefault(item => item.NotificationId.ToString() == notificationId && item.TenantId == tenantId);
            if (entry is null)
            {
                return null;
            }

            var acknowledged = entry with { Acknowledged = true, AcknowledgedAtUtc = DateTimeOffset.UtcNow };
            notifications.Remove(entry);
            notifications.Add(acknowledged);
            return acknowledged;
        }
    }
}

public sealed record NotificationEntry(
    Guid NotificationId,
    string TenantId,
    string RecipientUserId,
    string Subject,
    string Message,
    NotificationType Type,
    bool Acknowledged,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId,
    string? EmailAddress = null,
    DateTimeOffset? AcknowledgedAtUtc = null);

public sealed record CreateNotificationRequest(
    string TenantId,
    string RecipientUserId,
    string Subject,
    string Message);

public sealed record CreateEmailNotificationRequest(
    string TenantId,
    string RecipientUserId,
    string EmailAddress,
    string Subject,
    string Message);

public enum NotificationType
{
    InApp,
    Email
}