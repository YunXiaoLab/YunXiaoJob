using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.Interfaces.Repositories;

public interface IJobPostingRepository
{
    Task<JobPosting?> GetByIdAsync(Guid jobPostingId, bool includeSkills = false,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobPosting>> GetPublishedAsync(string? keyword, string? location,
        EmploymentType? employmentType, WorkplaceType? workplaceType,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobPosting>> GetAllAsync(JobPostingStatus? status,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobPosting>> GetByIdsAsync(IReadOnlyCollection<Guid> jobPostingIds,
        CancellationToken cancellationToken = default);
    Task AddAsync(JobPosting jobPosting, CancellationToken cancellationToken = default);
    Task AddSkillAsync(JobPostingSkill skill, CancellationToken cancellationToken = default);
    void RemoveSkill(JobPostingSkill skill);
    Task<IReadOnlyList<JobPosting>> GetByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
}
