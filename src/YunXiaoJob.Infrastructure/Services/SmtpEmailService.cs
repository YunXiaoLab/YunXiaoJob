using System.Net;
using System.Net.Mail;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Enums;
using YunXiaoJob.Infrastructure.Settings;
namespace YunXiaoJob.Infrastructure.Services;
public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings; public SmtpEmailService(SmtpSettings settings) => _settings = settings;
    public Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        var link = $"{_settings.ResetPasswordUrl}?token={Uri.EscapeDataString(resetToken)}";
        return SendAsync(email, "Reset your YunXiaoJob password", $"Use this link to reset your password: {link}", cancellationToken);
    }
    public Task SendRegistrationOtpAsync(string email, string otp, CancellationToken cancellationToken = default) =>
        SendAsync(email, "Xác thực đăng ký YunXiaoJob", $"Mã xác thực của bạn là: {otp}. Mã có hiệu lực trong 5 phút.", cancellationToken);
    public Task SendCompanyAccountCredentialsAsync(string email, string fullName, string companyName,
        CompanyMemberRole role, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        var roleLabel = role switch { CompanyMemberRole.Owner => "Chủ doanh nghiệp", CompanyMemberRole.HR => "Nhân sự (HR)", _ => "Chuyên viên tuyển dụng (Recruiter)" };
        var body = $"Chào {fullName},\n\nTài khoản {roleLabel} của công ty \"{companyName}\" trên YunXiaoJob đã được tạo.\n\n"
            + $"Email đăng nhập: {email}\nMật khẩu tạm thời: {temporaryPassword}\n\n"
            + "Vui lòng đăng nhập và đổi mật khẩu ngay sau lần đăng nhập đầu tiên.";
        return SendAsync(email, $"Tài khoản YunXiaoJob của {companyName}", body, cancellationToken);
    }
    public Task SendCompanyRegistrationRejectedAsync(string email, string contactFullName, string companyName,
        string? reason, CancellationToken cancellationToken = default)
    {
        var body = $"Chào {contactFullName},\n\nRất tiếc, đăng ký tài khoản công ty \"{companyName}\" trên YunXiaoJob chưa được duyệt.\n\n"
            + $"Lý do: {(string.IsNullOrWhiteSpace(reason) ? "Hồ sơ chưa đáp ứng điều kiện xét duyệt." : reason)}\n\n"
            + "Bạn có thể bổ sung thông tin và gửi lại đăng ký.";
        return SendAsync(email, $"Đăng ký công ty {companyName} chưa được duyệt", body, cancellationToken);
    }
    private async Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken)
    {
        using var message = new MailMessage(_settings.FromEmail, email, subject, body) { IsBodyHtml = false };
        using var client = new SmtpClient(_settings.Host, _settings.Port) { EnableSsl = _settings.EnableSsl, Credentials = new NetworkCredential(_settings.Username, _settings.Password) };
        await client.SendMailAsync(message, cancellationToken);
    }
}
