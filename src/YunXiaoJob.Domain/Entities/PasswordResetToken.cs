using YunXiaoJob.Domain.Common;
namespace YunXiaoJob.Domain.Entities;
public class PasswordResetToken : Entity { public Guid UserId { get; set; } public string TokenHash { get; set; } = string.Empty; public DateTime ExpiresAtUtc { get; set; } public DateTime? UsedAtUtc { get; set; } }
