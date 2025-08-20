using Library.Core.ServiceContract;
using LibraTrack.API.DTOs;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraTrack.API.Controllers
{
    [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [Authorize(Roles ="Admin")]
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [HttpPost("sendNotification")]
        public async Task<ActionResult> SendNotificationAsync([FromBody] CreateNotificationDto model)
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Message))
                return BadRequest("Invalid notification parameters.");
            var res = await _notificationService.SendNotificationAsync(model.UserId, model.Title, model.Message);
            if (!res)
                return NotFound("User not found or notification could not be sent.");
            return Ok(new { Message = "Notification sent successfully." });
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("UserNotification")]
        public async Task<ActionResult> GetUserNotificationsAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var notifications = await _notificationService.GetUserNotificationsAsync(userEmail);
            if (notifications == null || !notifications.Any())
                return NotFound("No notifications found for the user.");
            return Ok(notifications);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpPut("{id}/MarkAsRead")]
        public async Task<ActionResult> MarkAsReadAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _notificationService.GetByIdAsync(id);
            var result = await _notificationService.MarkAsReadAsync(id);
            if (!result)
                return NotFound("Notification not found or could not be marked as read.");
            if(notification?.UserId != userId)
                return StatusCode(403, new ApiResponse(403, "You do not have permission to mark this notification as read"));
            return Ok(new { Message = "Notification marked as read successfully." });
        }
    }
}
