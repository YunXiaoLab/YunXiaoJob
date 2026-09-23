using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class CandidateEducation : Entity
{
    public Guid CandidateProfileId { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string? Major { get; set; }
    public EducationLevel Level { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public decimal? Gpa { get; set; }
    public decimal? GpaScale { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}
