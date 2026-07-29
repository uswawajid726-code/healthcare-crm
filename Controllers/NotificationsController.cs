using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for user notifications and alerts.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves unread notifications for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the targeted user.</param>
        /// <returns>A list of unread notifications.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetNotifications([FromQuery] int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving notifications: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates a new notification alert.
        /// </summary>
        /// <param name="notification">Notification payload.</param>
        /// <returns>The created notification entity.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid notification payload."
                    });
                }

                notification.CreatedAt = DateTime.UtcNow;
                notification.IsRead = false;

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return StatusCode(201, notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating notification: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="id">The unique notification ID.</param>
        /// <returns>The updated notification.</returns>
        [HttpPatch("{id}/read")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Notification with ID {id} not found."
                    });
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while marking notification as read: {ex.Message}"
                });
            }
        }
    }
}