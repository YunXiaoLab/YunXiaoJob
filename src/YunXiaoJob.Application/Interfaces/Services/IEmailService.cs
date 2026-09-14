namespace YunXiaoJob.Application.Interfaces.Services;
public interface IEmailService { Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default); Task SendRegistrationOtpAsync(string email, string otp, CancellationToken cancellationToken = default); }
