using YunXiaoJob.Domain.Common;

namespace YunXiaoJob.Domain.Entities;

public class Resume : Entity
{
    public Guid CandidateProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
