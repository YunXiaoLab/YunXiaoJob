using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class CandidateProfile : Entity
{
    public Guid UserId { get; set; }
    public string? Headline { get; set; }
    public string? Summary { get; set; }
    public string? Location { get; set; }
    public int? YearsOfExperience { get; set; }
    public bool IsSearchable { get; set; }
    public string? AvatarUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.Unspecified;
    public decimal? ExpectedMinSalary { get; set; }
    public decimal? ExpectedMaxSalary { get; set; }
    public string? ExpectedSalaryCurrency { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    public ICollection<CandidateEducation> Educations { get; set; } = new List<CandidateEducation>();
    public ICollection<CandidateExperience> Experiences { get; set; } = new List<CandidateExperience>();
    public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
}
