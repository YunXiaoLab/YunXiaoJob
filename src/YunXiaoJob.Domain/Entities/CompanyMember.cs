using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class CompanyMember : Entity
{
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public CompanyMemberRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}
