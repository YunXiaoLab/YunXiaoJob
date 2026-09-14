using YunXiaoJob.Domain.Common;
namespace YunXiaoJob.Domain.Entities;
public class JobCategory : Entity { public string Name { get; set; } = string.Empty; public bool IsActive { get; set; } = true; }
