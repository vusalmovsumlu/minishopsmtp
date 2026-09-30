namespace NotificationService.API.Services;

public interface IEmailSender
{
    Task SendOrderConfirmationAsync(
        string toEmail,
        int orderId,
        string productName,
        int quantity,
        decimal totalPrice);
}
