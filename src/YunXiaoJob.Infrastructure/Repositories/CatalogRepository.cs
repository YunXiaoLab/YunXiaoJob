using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Infrastructure.Data;
namespace YunXiaoJob.Infrastructure.Repositories;
public class CatalogRepository : ICatalogRepository
{
    private readonly ApplicationDbContext _context; public CatalogRepository(ApplicationDbContext context) => _context = context;
    public async Task<IReadOnlyList<JobCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default) => await _context.JobCategories.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default) => await _context.Skills.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<Location>> GetLocationsAsync(CancellationToken cancellationToken = default) => await _context.Locations.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public Task AddCategoryAsync(JobCategory category, CancellationToken cancellationToken = default) => _context.JobCategories.AddAsync(category, cancellationToken).AsTask();
    public Task AddSkillAsync(Skill skill, CancellationToken cancellationToken = default) => _context.Skills.AddAsync(skill, cancellationToken).AsTask();
    public Task AddLocationAsync(Location location, CancellationToken cancellationToken = default) => _context.Locations.AddAsync(location, cancellationToken).AsTask();
}
