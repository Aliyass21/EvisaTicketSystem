using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Hubs;

namespace EVisaTicketSystem.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Sends a notification to all connected clients
        public async Task SendNotificationToAllAsync(Notification notification)
        {
            try
            {
                // Ensure new ID is assigned if not already set
                if (notification.Id == Guid.Empty)
                {
                    notification.Id = Guid.NewGuid();
                }
                
                // Store in database
                _unitOfWork.NotificationRepository.Add(notification);
                await _unitOfWork.Complete();
                
                // Send to all connected clients
                await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", new
                {
                    Id = notification.Id,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    UserId = notification.UserId,
                    DateCreated = notification.DateCreated
                });
                
                _logger.LogInformation($"Global notification sent successfully (ID: {notification.Id})");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send global notification: {ex.Message}");
                throw;
            }
        }

        // Alias for SendNotificationToAllAsync for backward compatibility
        public async Task SendGlobalNotificationAsync(Notification notification)
        {
            await SendNotificationToAllAsync(notification);
        }

        // Sends a notification to a specific user
        public async Task SendNotificationAsync(Notification notification)
        {
            try 
            {
                // Ensure new ID is assigned if not already set
                if (notification.Id == Guid.Empty)
                {
                    notification.Id = Guid.NewGuid();
                }
                
                // If DateCreated isn't set, set it to now
                if (notification.DateCreated == default)
                {
                    notification.DateCreated = DateTime.UtcNow;
                }
                
                // Store notification in DB (persists even if user is offline)
                _unitOfWork.NotificationRepository.Add(notification);
                await _unitOfWork.Complete();
                
                // Try to send real-time if user is connected
                await _hubContext.Clients.User(notification.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        Id = notification.Id,
                        Message = notification.Message,
                        IsRead = notification.IsRead,
                        UserId = notification.UserId,
                        DateCreated = notification.DateCreated
                    });
                    
                _logger.LogInformation($"Notification sent to user {notification.UserId} (ID: {notification.Id})");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notification: {ex.Message} | Stack: {ex.StackTrace}");
                throw;
            }
        }

        // Mark a single notification as read (client-side only version)
        public async Task NotifyClientNotificationReadAsync(Guid userId, Guid notificationId)
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("NotificationMarkedAsRead", new
                {
                    NotificationId = notificationId,
                    IsRead = true
                });
        }
        
        // Mark a single notification as read (database update + client notification)
        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            try
            {
                // 1. Fetch the notification from DB
                var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
                if (notification == null)
                    throw new Exception($"Notification not found with ID {notificationId}");

                // 2. Update the IsRead field
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    await _unitOfWork.NotificationRepository.UpdateAsync(notification);
                    await _unitOfWork.Complete();
                }

                // 3. Broadcast to the user that it's now read
                await _hubContext.Clients.User(notification.UserId.ToString()).SendAsync(
                    "NotificationMarkedAsRead",
                    new { NotificationId = notification.Id, IsRead = true }
                );
                
                _logger.LogInformation($"Notification {notificationId} marked as read for user {notification.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to mark notification as read: {ex.Message}");
                throw;
            }
        }

        // Mark all notifications for a user as read
        public async Task MarkAllNotificationsAsReadAsync(Guid userId)
        {
            try
            {
                // 1. Get all notifications for this user
                var notifications = await _unitOfWork.NotificationRepository.GetNotificationsForUserAsync(userId);

                // 2. Mark them all as read
                bool anyChanges = false;
                foreach (var notification in notifications)
                {
                    if (!notification.IsRead)
                    {
                        notification.IsRead = true;
                        await _unitOfWork.NotificationRepository.UpdateAsync(notification);
                        anyChanges = true;
                    }
                }

                // 3. Save changes to DB (only if something changed)
                if (anyChanges)
                {
                    await _unitOfWork.Complete();
                }

                // 4. Notify the user on the client side that all are now read
                await _hubContext.Clients.User(userId.ToString())
                    .SendAsync("AllNotificationsMarkedAsRead");
                    
                _logger.LogInformation($"All notifications marked as read for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to mark all notifications as read: {ex.Message}");
                throw;
            }
        }
    }
}