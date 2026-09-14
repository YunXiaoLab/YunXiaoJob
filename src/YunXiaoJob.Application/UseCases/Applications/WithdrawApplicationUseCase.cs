using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class WithdrawApplicationUseCase
{
    private readonly ICandidateRepository _candidates; private readonly IJobApplicationRepository _applications; private readonly IUnitOfWork _unitOfWork;
    public WithdrawApplicationUseCase(ICandidateRepository candidates, IJobApplicationRepository applications, IUnitOfWork unitOfWork) { _candidates = candidates; _applications = applications; _unitOfWork = unitOfWork; }
    public async Task<JobApplicationResponse> ExecuteAsync(Guid applicationId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var application = await _applications.GetByIdAsync(applicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        if (application.CandidateProfileId != profile.Id) throw new UnauthorizedAccessException();
        if (application.Status is ApplicationStatus.Hired or ApplicationStatus.Rejected or ApplicationStatus.Withdrawn) throw new InvalidOperationException("Application cannot be withdrawn.");
        var previous = application.Status; application.Status = ApplicationStatus.Withdrawn; application.UpdatedAtUtc = DateTime.UtcNow;
        await _applications.AddStatusHistoryAsync(new ApplicationStatusHistory { JobApplicationId = application.Id, FromStatus = previous, ToStatus = application.Status, Note = "Withdrawn by candidate." }, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobApplicationResponse(application.Id, application.JobPostingId, application.CandidateProfileId, application.ResumeId, application.AssignedRecruiterMemberId, application.CoverLetter, application.Status, application.SubmittedAtUtc);
    }
}
