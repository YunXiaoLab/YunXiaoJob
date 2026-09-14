using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.JobPostings;

public class SubmitJobPostingUseCase
{
    private readonly ICompanyRepository _companies; private readonly IJobPostingRepository _jobs; private readonly IUnitOfWork _unitOfWork;
    public SubmitJobPostingUseCase(ICompanyRepository companies, IJobPostingRepository jobs, IUnitOfWork unitOfWork) { _companies = companies; _jobs = jobs; _unitOfWork = unitOfWork; }
    public async Task<JobPostingResponse> ExecuteAsync(Guid jobPostingId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var job = await _jobs.GetByIdAsync(jobPostingId, true, cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive || member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR)) throw new UnauthorizedAccessException();
        if (job.Status is not (JobPostingStatus.Draft or JobPostingStatus.Rejected)) throw new InvalidOperationException("Only draft or rejected postings can be submitted.");
        job.Status = JobPostingStatus.PendingApproval; job.UpdatedAtUtc = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobPostingResponse(job.Id, job.CompanyId, job.CreatedByMemberId, job.AssignedRecruiterMemberId, job.Title, job.Description, job.Requirements, job.Benefits, job.Location, job.EmploymentType, job.WorkplaceType, job.MinSalary, job.MaxSalary, job.Currency, job.ApplicationDeadline, job.Status, job.PublishedAtUtc, job.Skills.Select(x => x.Name).ToList());
    }
}
