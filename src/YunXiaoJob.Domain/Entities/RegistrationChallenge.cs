using YunXiaoJob.Domain.Common;
namespace YunXiaoJob.Domain.Entities;
public class RegistrationChallenge : Entity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime LastSentAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public int ResendCount { get; set; }
}
