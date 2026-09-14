using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces.Repositories;

namespace YunXiaoJob.Application.UseCases.JobPostings;

public class SearchJobPostingsUseCase
{
    private readonly IJobPostingRepository _jobs;
    public SearchJobPostingsUseCase(IJobPostingRepository jobs) => _jobs = jobs;
    public async Task<IReadOnlyList<JobPostingResponse>> ExecuteAsync(SearchJobPostingsRequest request, CancellationToken cancellationToken = default)
    {
        var jobs = await _jobs.GetPublishedAsync(request.Keyword, request.Location, request.EmploymentType, request.WorkplaceType, cancellationToken);
        return jobs.Select(x => new JobPostingResponse(x.Id, x.CompanyId, x.CreatedByMemberId, x.AssignedRecruiterMemberId, x.Title, x.Description, x.Requirements, x.Benefits, x.Location, x.EmploymentType, x.WorkplaceType, x.MinSalary, x.MaxSalary, x.Currency, x.ApplicationDeadline, x.Status, x.PublishedAtUtc, x.Skills.Select(s => s.Name).ToList())).ToList();
    }
}
