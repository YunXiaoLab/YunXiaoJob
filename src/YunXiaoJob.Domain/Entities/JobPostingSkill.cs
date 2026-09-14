using YunXiaoJob.Domain.Common;

namespace YunXiaoJob.Domain.Entities;

public class JobPostingSkill : Entity
{
    public Guid JobPostingId { get; set; }
    public string Name { get; set; } = string.Empty;
}
