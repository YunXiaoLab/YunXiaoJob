using YunXiaoJob.Domain.Common;

namespace YunXiaoJob.Domain.Entities;

public class CandidateProfile : Entity
{
    public Guid UserId { get; set; }
    public string? Headline { get; set; }
    public string? Summary { get; set; }
    public string? Location { get; set; }
    public int? YearsOfExperience { get; set; }
    public bool IsSearchable { get; set; }
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}
