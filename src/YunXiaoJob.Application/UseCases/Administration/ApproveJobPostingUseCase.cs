using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Administration;

public class ApproveJobPostingUseCase
{
    private readonly IUserRepository _users; private readonly IJobPostingRepository _jobs; private readonly IUnitOfWork _unitOfWork;
    public ApproveJobPostingUseCase(IUserRepository users, IJobPostingRepository jobs, IUnitOfWork unitOfWork) { _users = users; _jobs = jobs; _unitOfWork = unitOfWork; }
    public async Task<JobPostingResponse> ExecuteAsync(Guid jobPostingId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken) ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin) throw new UnauthorizedAccessException("Only an admin can approve a job posting.");
        var job = await _jobs.GetByIdAsync(jobPostingId, true, cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        if (job.Status != JobPostingStatus.PendingApproval) throw new InvalidOperationException("Only submitted postings can be approved.");
        job.Status = JobPostingStatus.Published; job.PublishedAtUtc = DateTime.UtcNow; job.UpdatedAtUtc = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobPostingResponse(job.Id, job.CompanyId, job.CreatedByMemberId, job.AssignedRecruiterMemberId, job.Title, job.Description, job.Requirements, job.Benefits, job.Location, job.EmploymentType, job.WorkplaceType, job.MinSalary, job.MaxSalary, job.Currency, job.ApplicationDeadline, job.Status, job.PublishedAtUtc, job.Skills.Select(x => x.Name).ToList());
    }
}
