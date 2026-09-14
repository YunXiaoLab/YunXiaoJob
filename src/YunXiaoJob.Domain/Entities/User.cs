using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class User : Entity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public PlatformRole PlatformRole { get; set; } = PlatformRole.User;
    public bool IsActive { get; set; } = true;
    public CandidateProfile? CandidateProfile { get; set; }
}
