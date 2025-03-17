using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        INotificationRepository notificationRepository) // <-- new
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
    }

    // Sends a notification to all connected clients.
public async Task SendNotificationToAllAsync(Notification notification)
{
    await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", new
    {
        Message = notification.Message,
        IsRead = notification.IsRead,
        UserId = notification.UserId
    });
}
    // Broadcast a global notification to all connected clients.
 public async Task SendGlobalNotificationAsync(Notification notification)
{
    await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", new
    {
        Message = notification.Message,
        IsRead = notification.IsRead,
        UserId = notification.UserId
    });
}


    // Sends a notification to a specific user.
    public async Task SendNotificationAsync(Notification notification)
    {
        await _hubContext.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", new
        {
            Message = notification.Message,
            IsRead = notification.IsRead,
            UserId = notification.UserId
        });
    }
    public async Task MarkNotificationAsReadAsync(Guid userId, Guid notificationId)
{
    // This only sends a SignalR message to the client indicating
    // the notification is read. No DB update is performed here.
    await _hubContext.Clients
                     .User(userId.ToString())
                     .SendAsync("NotificationMarkedAsRead", new
    {
        NotificationId = notificationId,
        IsRead = true
    });
}
    // New: Mark a single notification as read
public async Task MarkNotificationAsReadAsync(Guid notificationId)
{
    // 1. Fetch the notification from DB
    var notification = await _notificationRepository.GetByIdAsync(notificationId);
    if (notification == null)
        throw new Exception("Notification not found.");

    // 2. Update the IsRead field
    if (!notification.IsRead)
    {
        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
    }

    // 3. Optionally broadcast to the user that it's now read
    await _hubContext.Clients.User(notification.UserId.ToString()).SendAsync(
        "NotificationMarkedAsRead",
        new { NotificationId = notification.Id, IsRead = true }
    );
}


    // (Optional) Mark all notifications for a user as read
    public async Task MarkAllNotificationsAsReadAsync(Guid userId)
    {
        // 1. Get all notifications for this user
        var notifications = await _notificationRepository.GetNotificationsForUserAsync(userId);

        // 2. Mark them all as read in a loop
        bool anyChanges = false;
        foreach (var notif in notifications)
        {
            if (!notif.IsRead)
            {
                notif.IsRead = true;
                anyChanges = true;
            }
        }

        // 3. Save changes to DB (only if something changed)
        if (anyChanges)
        {
            // If you have an UpdateRangeAsync, use that. Otherwise, just loop or rely on EF’s tracking:
            //_context.SaveChanges(); (depends on your pattern)
            foreach (var n in notifications)
            {
                await _notificationRepository.UpdateAsync(n);
            }
        }

        // 4. Optionally notify the user on the client side that all are now read
        await _hubContext.Clients.User(userId.ToString())
                         .SendAsync("AllNotificationsMarkedAsRead");
    }

    
}
