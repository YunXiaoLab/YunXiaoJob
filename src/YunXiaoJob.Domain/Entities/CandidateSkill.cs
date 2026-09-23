using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class CandidateSkill : Entity
{
    public Guid CandidateProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillProficiency Proficiency { get; set; }
    public int? YearsOfExperience { get; set; }
    public int? LastUsedYear { get; set; }
}
