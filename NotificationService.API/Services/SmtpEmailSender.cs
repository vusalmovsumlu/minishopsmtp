using System.Net;
using System.Net.Mail;

namespace NotificationService.API.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        IConfiguration config,
        ILogger<SmtpEmailSender> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(
        string toEmail,
        int orderId,
        string productName,
        int quantity,
        decimal totalPrice)
    {
        var host = _config["Smtp:Host"] ?? "smtp.gmail.com";
        var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
        var username = _config["Smtp:Username"] ?? string.Empty;
        var password = _config["Smtp:Password"] ?? string.Empty;
        var fromEmail = _config["Smtp:FromEmail"] ?? username;
        var fromName = _config["Smtp:FromName"] ?? "MiniShop";
        var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) ? ssl : true;

        // Əgər SMTP login hələ qurulmayıbsa — crash vermə, sadəcə console-a yaz.
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning(
                "SMTP credentials not configured. Email to {Email} skipped. Order #{OrderId}",
                toEmail, orderId);

            Console.WriteLine("----------------------------------");
            Console.WriteLine("NEW NOTIFICATION (SMTP not configured, mail NOT sent)");
            Console.WriteLine($"To: {toEmail}");
            Console.WriteLine($"Order Id: {orderId}");
            Console.WriteLine($"Product: {productName}");
            Console.WriteLine($"Quantity: {quantity}");
            Console.WriteLine($"Total: {totalPrice} AZN");
            Console.WriteLine("----------------------------------");
            return;
        }

        var subject = $"MiniShop — Sifariş #{orderId} təsdiqləndi ✅";

        var body = $"""
            Salam!

            Sifarişiniz uğurla yaradıldı:

            Sifariş №: {orderId}
            Məhsul: {productName}
            Say: {quantity}
            Cəmi: {totalPrice} AZN

            Bizi seçdiyiniz üçün təşəkkürlər!
            — MiniShop komandası
            """;

        using var message = new MailMessage();
        message.From = new MailAddress(fromEmail, fromName);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = false;

        using var client = new SmtpClient(host, port);
        client.EnableSsl = enableSsl;
        client.Credentials = new NetworkCredential(username, password);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;

        await client.SendMailAsync(message);

        _logger.LogInformation(
            "Order confirmation mail sent to {Email} for order {OrderId}",
            toEmail, orderId);
    }
}
