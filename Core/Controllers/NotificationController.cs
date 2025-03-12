using Microsoft.AspNetCore.Mvc;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Hubs;
using EVisaTicketSystem.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(NotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<IActionResult> PostNotification([FromBody] NotificationDto notificationDto)
        {
            // Create a new Notification entity
            var notification = new Notification
            {
                Message = notificationDto.Message,
                IsRead = false, // default on creation
                UserId = notificationDto.UserId
            };

            // Using the unit of work to add and persist the notification.
            // Make sure INotificationRepository includes an Add method.
            _unitOfWork.NotificationRepository.Add(notification);
            var result = await _unitOfWork.Complete();
            if (!result)
            {
                return StatusCode(500, "Failed to save the notification");
            }

            // Use SignalR NotificationService to send the notification.
            if (notification.UserId == Guid.Empty)
            {
                await _notificationService.SendNotificationToAllAsync(notification);
            }
            else
            {
                await _notificationService.SendNotificationAsync(notification);
            }

            return Ok(notification);
        }
    }

    // DTO for incoming notification data.
    public class NotificationDto
    {
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
