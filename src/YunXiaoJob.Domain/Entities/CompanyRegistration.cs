using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class CompanyRegistration : Entity
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public int? EmployeeCount { get; set; }
    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhoneNumber { get; set; }
    public CompanyRegistrationStatus Status { get; set; } = CompanyRegistrationStatus.Pending;
    public string? ReviewNote { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public Guid? CreatedCompanyId { get; set; }
    public Guid? CreatedOwnerUserId { get; set; }
}
