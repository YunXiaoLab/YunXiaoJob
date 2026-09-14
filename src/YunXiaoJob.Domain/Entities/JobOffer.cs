using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class JobOffer : Entity
{
    public Guid JobApplicationId { get; set; }
    public Guid CreatedByMemberId { get; set; }
    public decimal? Salary { get; set; }
    public string Currency { get; set; } = "VND";
    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpiresOn { get; set; }
    public string? Note { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Draft;
}
