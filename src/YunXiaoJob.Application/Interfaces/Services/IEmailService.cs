using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default);
    Task SendRegistrationOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task SendCompanyAccountCredentialsAsync(string email, string fullName, string companyName,
        CompanyMemberRole role, string temporaryPassword, CancellationToken cancellationToken = default);
    Task SendCompanyRegistrationRejectedAsync(string email, string contactFullName, string companyName,
        string? reason, CancellationToken cancellationToken = default);
}
