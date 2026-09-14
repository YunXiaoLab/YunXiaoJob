using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class ApplicationStatusHistory : Entity
{
    public Guid JobApplicationId { get; set; }
    public ApplicationStatus? FromStatus { get; set; }
    public ApplicationStatus ToStatus { get; set; }
    public Guid? ChangedByMemberId { get; set; }
    public string? Note { get; set; }
}
