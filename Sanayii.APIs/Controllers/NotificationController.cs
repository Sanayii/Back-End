using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sanayii.APIs.Controllers;
using Sanayii.Core.Entities;
using Sanayii.Service.Hubs;


[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<ChatController> _logger;

    public NotificationController(IHubContext<NotificationHub> hubContext, ILogger<ChatController> _logger)
    {
        _hubContext = hubContext;
        this._logger = _logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] Notification notification)
    {
        if (notification == null)
        {
            return BadRequest("Notification data is required.");
        }

        // Broadcast the notification to client
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);

        // Log the notification action
        _logger.LogInformation($"Notification sent to CustomerId={notification.UserId}: Your service request status has been updated to: {notification.Content}");

        return Ok();
    }
}
