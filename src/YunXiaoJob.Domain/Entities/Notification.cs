using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;
namespace YunXiaoJob.Domain.Entities;
public class Notification : Entity { public Guid UserId { get; set; } public string Title { get; set; } = string.Empty; public string Content { get; set; } = string.Empty; public NotificationType Type { get; set; } public string? TargetUrl { get; set; } public bool IsRead { get; set; } }
