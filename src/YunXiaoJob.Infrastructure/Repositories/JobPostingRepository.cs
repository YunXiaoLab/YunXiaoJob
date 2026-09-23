using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;
using YunXiaoJob.Infrastructure.Data;

namespace YunXiaoJob.Infrastructure.Repositories;

public class JobPostingRepository : IJobPostingRepository
{
    private readonly ApplicationDbContext _context;
    public JobPostingRepository(ApplicationDbContext context) => _context = context;

    public Task<JobPosting?> GetByIdAsync(Guid jobPostingId, bool includeSkills = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<JobPosting> query = _context.JobPostings;
        if (includeSkills) query = query.Include(x => x.Skills);
        return query.FirstOrDefaultAsync(x => x.Id == jobPostingId, cancellationToken);
    }

    public async Task<IReadOnlyList<JobPosting>> GetPublishedAsync(string? keyword, string? location,
        EmploymentType? employmentType, WorkplaceType? workplaceType,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        IQueryable<JobPosting> query = _context.JobPostings
            .Include(x => x.Skills)
            .Where(x => x.Status == JobPostingStatus.Published &&
                (!x.ApplicationDeadline.HasValue || x.ApplicationDeadline >= today));

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim();
            query = query.Where(x => x.Title.Contains(term) || x.Description.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var term = location.Trim();
            query = query.Where(x => x.Location.Contains(term));
        }

        if (employmentType.HasValue) query = query.Where(x => x.EmploymentType == employmentType);
        if (workplaceType.HasValue) query = query.Where(x => x.WorkplaceType == workplaceType);

        return await query.OrderByDescending(x => x.PublishedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobPosting>> GetAllAsync(JobPostingStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<JobPosting> query = _context.JobPostings.Include(x => x.Skills);
        if (status is not null) query = query.Where(x => x.Status == status);
        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobPosting>> GetByIdsAsync(IReadOnlyCollection<Guid> jobPostingIds,
        CancellationToken cancellationToken = default) =>
        jobPostingIds.Count == 0
            ? []
            : await _context.JobPostings.Include(x => x.Skills)
                .Where(x => jobPostingIds.Contains(x.Id)).ToListAsync(cancellationToken);

    public Task AddAsync(JobPosting jobPosting, CancellationToken cancellationToken = default) =>
        _context.JobPostings.AddAsync(jobPosting, cancellationToken).AsTask();

    public Task AddSkillAsync(JobPostingSkill skill, CancellationToken cancellationToken = default) =>
        _context.JobPostingSkills.AddAsync(skill, cancellationToken).AsTask();

    public void RemoveSkill(JobPostingSkill skill) => _context.JobPostingSkills.Remove(skill);
    public async Task<IReadOnlyList<JobPosting>> GetByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default) => await _context.JobPostings.Where(x => x.CompanyId == companyId).Include(x => x.Skills).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
}
