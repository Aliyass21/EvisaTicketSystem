using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Sends a notification to all connected clients.
public async Task SendNotificationToAllAsync(Notification notification)
{
    await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
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
}
