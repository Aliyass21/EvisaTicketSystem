// using EVisaTicketSystem.Core.Controllers;
// using EVisaTicketSystem.Core.DTOs;
// using EVisaTicketSystem.Core.Entities;
// using EVisaTicketSystem.Core.Interfaces;
// using EVisaTicketSystem.Hubs;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Threading.Tasks;

// namespace EVisaTicketSystem.API.Controllers
// {
//     public class NotificationsController : BaseApiController
//     {
//         private readonly IUnitOfWork _unitOfWork;
//         private readonly NotificationService _notificationService;

//         public NotificationsController(IUnitOfWork unitOfWork, NotificationService notificationService)
//         {
//             _unitOfWork = unitOfWork;
//             _notificationService = notificationService;
//         }

//         // POST: api/Notifications/broadcast
// [HttpPost("broadcast")]
// public async Task<IActionResult> BroadcastNotification([FromBody] NotificationDto dto)
// {
//     // Replace with the actual system user id from your database.
//     var systemUserId = new Guid("ea6ad250-87ee-4522-8c31-9b69dfba036e");

//     var notification = new Notification
//     {
//         Message = dto.Message,
//         IsRead = dto.IsRead,
//         UserId = systemUserId 
//     };

//     // Optionally persist the notification
//     await _unitOfWork.NotificationRepository.CreateAsync(notification);
//     await _unitOfWork.Complete();

//     await _notificationService.SendNotificationToAllAsync(notification);
//     return Ok(new { message = "Notification broadcasted successfully." });
// }


//         // POST: api/Notifications/user
//         [HttpPost("user")]
//         public async Task<IActionResult> SendNotificationToUser([FromBody] NotificationDto dto)
//         {
//             var notification = new Notification
//             {
//                 Message = dto.Message,
//                 IsRead = dto.IsRead,
//                 UserId = dto.UserId
//             };

//             await _unitOfWork.NotificationRepository.CreateAsync(notification);
//             await _unitOfWork.Complete();

//             await _notificationService.SendNotificationAsync(notification);
//             return Ok(new { message = "Notification sent to user successfully." });
//         }
//     }
// }
