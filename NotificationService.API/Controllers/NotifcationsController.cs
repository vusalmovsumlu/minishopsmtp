using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Models;
using NotificationService.API.Services;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IEmailSender _emailSender;

    public NotificationsController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    [HttpPost]
    public async Task<IActionResult> Send(NotificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");

        Console.WriteLine("----------------------------------");
        Console.WriteLine("NEW NOTIFICATION");
        Console.WriteLine($"To: {request.Email}");
        Console.WriteLine($"Order Id: {request.OrderId}");
        Console.WriteLine($"Product: {request.ProductName}");
        Console.WriteLine($"Quantity: {request.Quantity}");
        Console.WriteLine($"Total: {request.TotalPrice} AZN");
        Console.WriteLine("----------------------------------");

        await _emailSender.SendOrderConfirmationAsync(
            request.Email.Trim(),
            request.OrderId,
            request.ProductName,
            request.Quantity,
            request.TotalPrice);

        return Ok(new
        {
            message = "Notification sent successfully."
        });
    }
}