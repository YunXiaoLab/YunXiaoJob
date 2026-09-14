using YunXiaoJob.Domain.Common;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Domain.Entities;

public class Company : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? Industry { get; set; }
    public int? EmployeeCount { get; set; }
    public CompanyStatus Status { get; set; } = CompanyStatus.PendingVerification;
    public ICollection<CompanyMember> Members { get; set; } = new List<CompanyMember>();
}
