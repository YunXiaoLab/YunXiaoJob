using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;
using YunXiaoJob.Infrastructure.Data;
namespace YunXiaoJob.Infrastructure.Repositories;
public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context; public DashboardRepository(ApplicationDbContext context) => _context = context;
    public async Task<(int Users, int Candidates, int Companies, int ActiveCompanies, int JobPostings, int PublishedJobPostings, int Applications)> GetPlatformCountsAsync(CancellationToken ct = default) => (await _context.Users.CountAsync(ct), await _context.CandidateProfiles.CountAsync(ct), await _context.Companies.CountAsync(ct), await _context.Companies.CountAsync(x => x.Status == CompanyStatus.Active, ct), await _context.JobPostings.CountAsync(ct), await _context.JobPostings.CountAsync(x => x.Status == JobPostingStatus.Published, ct), await _context.JobApplications.CountAsync(ct));
    public async Task<(int JobPostings, int PublishedJobPostings, int Applications, IReadOnlyDictionary<ApplicationStatus, int> ApplicationsByStatus)> GetCompanyCountsAsync(Guid companyId, CancellationToken ct = default)
    { var jobIds = _context.JobPostings.Where(x => x.CompanyId == companyId).Select(x => x.Id); var applications = _context.JobApplications.Where(x => jobIds.Contains(x.JobPostingId)); var grouped = await applications.GroupBy(x => x.Status).Select(x => new { x.Key, Count = x.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count, ct); return (await jobIds.CountAsync(ct), await _context.JobPostings.CountAsync(x => x.CompanyId == companyId && x.Status == JobPostingStatus.Published, ct), await applications.CountAsync(ct), grouped); }
}
