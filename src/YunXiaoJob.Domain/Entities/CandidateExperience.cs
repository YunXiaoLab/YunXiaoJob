using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class CandidateExperience : Entity
{
    public Guid CandidateProfileId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public EmploymentType? EmploymentType { get; set; }
    public string? Location { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}
