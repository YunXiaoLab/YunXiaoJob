using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class JobApplication : Entity
{
    public Guid JobPostingId { get; set; }
    public Guid CandidateProfileId { get; set; }
    public Guid ResumeId { get; set; }
    public Guid? AssignedRecruiterMemberId { get; set; }
    public string? CoverLetter { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<JobOffer> Offers { get; set; } = new List<JobOffer>();
}
