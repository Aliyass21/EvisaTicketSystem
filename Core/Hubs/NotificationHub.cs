using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public NotificationHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Method to get notifications for the authenticated user
// Update the NotificationHub method to use IEnumerable instead of List
public async Task<IEnumerable<Notification>> GetUserNotifications()
{
    try
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Extracted UserId from claims: {userIdClaim}");

        if (string.IsNullOrEmpty(userIdClaim))
        {
            Console.WriteLine("UserId claim is missing or invalid.");
            return Enumerable.Empty<Notification>();
        }

        if (!Guid.TryParse(userIdClaim, out Guid userGuid))
        {
            Console.WriteLine("Failed to parse UserId to Guid.");
            return Enumerable.Empty<Notification>();
        }

        var notifications = await _unitOfWork.NotificationRepository.GetNotificationsForUserAsync(userGuid);
        Console.WriteLine($"Found {notifications.Count()} notifications for user {userGuid}");

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