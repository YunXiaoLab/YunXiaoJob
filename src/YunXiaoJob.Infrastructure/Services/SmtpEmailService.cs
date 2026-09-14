using System.Net;
using System.Net.Mail;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Infrastructure.Settings;
namespace YunXiaoJob.Infrastructure.Services;
public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings; public SmtpEmailService(SmtpSettings settings) => _settings = settings;
    public async Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        var link = $"{_settings.ResetPasswordUrl}?token={Uri.EscapeDataString(resetToken)}";
        using var message = new MailMessage(_settings.FromEmail, email, "Reset your YunXiaoJob password", $"Use this link to reset your password: {link}") { IsBodyHtml = false };
        using var client = new SmtpClient(_settings.Host, _settings.Port) { EnableSsl = _settings.EnableSsl, Credentials = new NetworkCredential(_settings.Username, _settings.Password) };
        await client.SendMailAsync(message, cancellationToken);
    }
    public async Task SendRegistrationOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        using var message = new MailMessage(_settings.FromEmail, email, "Xác thực đăng ký YunXiaoJob", $"Mã xác thực của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");
        using var client = new SmtpClient(_settings.Host, _settings.Port) { EnableSsl = _settings.EnableSsl, Credentials = new NetworkCredential(_settings.Username, _settings.Password) };
        await client.SendMailAsync(message, cancellationToken);
    }
}
