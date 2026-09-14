using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class Interview : Entity
{
    public Guid JobApplicationId { get; set; }
    public Guid ScheduledByMemberId { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public string? LocationOrMeetingUrl { get; set; }
    public string? Note { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public string? EvaluationNote { get; set; }
    public int? Rating { get; set; }
}
