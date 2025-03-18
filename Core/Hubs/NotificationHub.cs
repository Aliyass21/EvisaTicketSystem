using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Hubs
{
    public class NotificationHub : Hub
    {
            private readonly NotificationService _notificationService;

        private readonly IUnitOfWork _unitOfWork;

        public NotificationHub(IUnitOfWork unitOfWork,NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;

        }
    // Called from the client to mark a specific notification as read
    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        // 1) Tell the NotificationService to update IsRead in the database
        await _notificationService.MarkNotificationAsReadAsync(notificationId);

        // 2) Optionally let the caller know it was processed successfully
        //    (You might skip this if your service method already broadcasts a message)
        await Clients.Caller.SendAsync("NotificationMarkedAsReadConfirmation", notificationId);
    }
    public async Task MarkAllNotificationsAsRead(Guid userId)
    {
        await _notificationService.MarkAllNotificationsAsReadAsync(userId);
    }

        // Method to get notifications for the authenticated user
// Update the NotificationHub method to use IEnumerable instead of List
public async Task<IEnumerable<Notification>> GetUserNotifications()
{
    try
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new Exception("User not authenticated");
        }
        var userId = Guid.Parse(userIdClaim);

        // Fetch notifications only for the logged-in user
        var notifications = await _unitOfWork.NotificationRepository.GetNotificationsForUserAsync(userId);

        Console.WriteLine($"Found {notifications.Count()} notifications for user {userId}");
        return notifications;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetUserNotifications: {ex.Message}");
        return Enumerable.Empty<Notification>();
    }
}


public override async Task OnConnectedAsync()
{
    await base.OnConnectedAsync();
    
    // Get the notifications
    var notifications = await GetUserNotifications();
    
    // Send the notifications to the connected client
    await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
}
    }
}