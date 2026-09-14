using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class ChangeApplicationStatusUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unitOfWork;
    public ChangeApplicationStatusUseCase(IJobApplicationRepository applications, IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unitOfWork) { _applications = applications; _jobs = jobs; _companies = companies; _unitOfWork = unitOfWork; }
    public async Task<JobApplicationResponse> ExecuteAsync(Guid applicationId, ChangeApplicationStatusRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var application = await _applications.GetByIdAsync(applicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        var job = await _jobs.GetByIdAsync(application.JobPostingId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        var permitted = member is not null && member.IsActive && (member.Role is CompanyMemberRole.Owner or CompanyMemberRole.HR || application.AssignedRecruiterMemberId == member.Id);
        if (!permitted) throw new UnauthorizedAccessException();
        if (application.Status is ApplicationStatus.Hired or ApplicationStatus.Rejected or ApplicationStatus.Withdrawn || request.Status is ApplicationStatus.Submitted or ApplicationStatus.Withdrawn) throw new InvalidOperationException("Invalid application status transition.");
        var previous = application.Status; application.Status = request.Status; application.UpdatedAtUtc = DateTime.UtcNow;
        await _applications.AddStatusHistoryAsync(new ApplicationStatusHistory { JobApplicationId = application.Id, FromStatus = previous, ToStatus = request.Status, ChangedByMemberId = member!.Id, Note = request.Note?.Trim() }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobApplicationResponse(application.Id, application.JobPostingId, application.CandidateProfileId, application.ResumeId, application.AssignedRecruiterMemberId, application.CoverLetter, application.Status, application.SubmittedAtUtc);
    }
}
