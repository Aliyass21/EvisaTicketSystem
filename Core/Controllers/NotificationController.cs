using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Hubs;
using EVisaTicketSystem.Core.Interfaces;
using System;
using System.Security.Claims;
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

        // POST: api/notifications/global
        // This endpoint is intended for admins to send a global notification.
        [HttpPost("global")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostGlobalNotification([FromBody] GlobalNotificationDto notificationDto)
        {
            // Extract the admin's user ID from claims.
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("Admin user ID not found.");
            }

            if (!Guid.TryParse(adminId, out Guid adminGuid))
            {
                return BadRequest("Invalid admin user ID.");
            }

            // Create the Notification entity with the admin's ID as the sender.
            var notification = new Notification
            {
                Message = notificationDto.Message,
                IsRead = false,
                UserId = adminGuid
            };

            // Persist the notification.
            _unitOfWork.NotificationRepository.Add(notification);
            var result = await _unitOfWork.Complete();
            if (!result)
            {
                return StatusCode(500, "Failed to save the global notification");
            }

            // Broadcast the notification to all connected clients using SendGlobalNotificationAsync.
            await _notificationService.SendGlobalNotificationAsync(notification);

            return Ok(notification);
        }
    }

    // DTO for global notifications.
    public class GlobalNotificationDto
    {
        public string Message { get; set; } = string.Empty;
    }
}
