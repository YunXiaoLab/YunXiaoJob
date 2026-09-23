using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.UseCases.Queries;

namespace YunXiaoJob.Application.UseCases.JobPostings;

public class SearchJobPostingsUseCase
{
    private readonly IJobPostingRepository _jobs;
    private readonly ICompanyRepository _companies;
    public SearchJobPostingsUseCase(IJobPostingRepository jobs, ICompanyRepository companies)
    { _jobs = jobs; _companies = companies; }

    public async Task<IReadOnlyList<JobPostingSummaryResponse>> ExecuteAsync(SearchJobPostingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var jobs = await _jobs.GetPublishedAsync(request.Keyword, request.Location, request.EmploymentType,
            request.WorkplaceType, cancellationToken);
        var companies = (await _companies.GetByIdsAsync(jobs.Select(x => x.CompanyId).Distinct().ToList(),
            cancellationToken)).ToDictionary(x => x.Id);
        return jobs.Select(x => x.ToSummary(companies.GetValueOrDefault(x.CompanyId))).ToList();
    }
}
