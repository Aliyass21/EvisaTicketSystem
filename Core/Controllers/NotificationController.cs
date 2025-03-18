using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Hubs;
using EVisaTicketSystem.Core.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EVisaTicketSystem.Core.Controllers;
using EVisaTicketSystem.Services;

namespace EVisaTicketSystem.Api.Controllers
{

    public class NotificationsController : BaseApiController
    {
        private readonly NotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(NotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

    [HttpPost("{notificationId}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
    {
        await _notificationService.MarkNotificationAsReadAsync(notificationId);
        return Ok();
    }

    [HttpPost("all/read")]
    public async Task<IActionResult> MarkAllNotificationsAsRead([FromBody] Guid userId)
    {
        await _notificationService.MarkAllNotificationsAsReadAsync(userId);
        return Ok();
    }

        // POST: api/notifications/global
        // This endpoint is intended for admins to send a global notification.
[HttpPost("global")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> PostGlobalNotification([FromBody] GlobalNotificationDto dto)
{
    var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(adminId, out Guid adminGuid))
        return BadRequest("Invalid admin user ID.");

    var notification = new Notification
    {
        Message = dto.Message,
        IsRead = false,
        UserId = adminGuid,
    };

    await _notificationService.SendGlobalNotificationAsync(notification);
    return Ok();
}

    }

    // DTO for global notifications.
    public class GlobalNotificationDto
    {
        public string Message { get; set; } = string.Empty;
    }
}
