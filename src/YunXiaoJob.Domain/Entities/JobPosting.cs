using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class JobPosting : Entity
{
    public Guid CompanyId { get; set; }
    public Guid? JobCategoryId { get; set; }
    public Guid CreatedByMemberId { get; set; }
    public Guid? AssignedRecruiterMemberId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string? Benefits { get; set; }
    public string Location { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public WorkplaceType WorkplaceType { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string Currency { get; set; } = "VND";
    public DateOnly? ApplicationDeadline { get; set; }
    public JobPostingStatus Status { get; set; } = JobPostingStatus.Draft;
    public DateTime? PublishedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public ICollection<JobPostingSkill> Skills { get; set; } = new List<JobPostingSkill>();
}
