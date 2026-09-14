using YunXiaoJob.Domain.Common;

namespace YunXiaoJob.Domain.Entities;

public class SavedJob : Entity
{
    public Guid CandidateProfileId { get; set; }
    public Guid JobPostingId { get; set; }
}
