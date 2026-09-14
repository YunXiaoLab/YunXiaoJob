using YunXiaoJob.Domain.Enums;
namespace YunXiaoJob.Application.Interfaces.Repositories;
public interface IDashboardRepository
{
    Task<(int Users, int Candidates, int Companies, int ActiveCompanies, int JobPostings, int PublishedJobPostings, int Applications)> GetPlatformCountsAsync(CancellationToken cancellationToken = default);
    Task<(int JobPostings, int PublishedJobPostings, int Applications, IReadOnlyDictionary<ApplicationStatus, int> ApplicationsByStatus)> GetCompanyCountsAsync(Guid companyId, CancellationToken cancellationToken = default);
}
