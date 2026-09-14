using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class AssignRecruiterUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unitOfWork;
    public AssignRecruiterUseCase(IJobApplicationRepository applications, IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unitOfWork) { _applications = applications; _jobs = jobs; _companies = companies; _unitOfWork = unitOfWork; }
    public async Task<JobApplicationResponse> ExecuteAsync(Guid applicationId, AssignRecruiterRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var application = await _applications.GetByIdAsync(applicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        var job = await _jobs.GetByIdAsync(application.JobPostingId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        var manager = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        if (manager is null || !manager.IsActive || manager.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR)) throw new UnauthorizedAccessException();
        var recruiter = await _companies.GetMemberByIdAsync(request.RecruiterMemberId, cancellationToken);
        if (recruiter is null || !recruiter.IsActive || recruiter.CompanyId != job.CompanyId || recruiter.Role != CompanyMemberRole.Recruiter) throw new InvalidOperationException("Assigned member must be an active recruiter of this company.");
        application.AssignedRecruiterMemberId = recruiter.Id; application.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobApplicationResponse(application.Id, application.JobPostingId, application.CandidateProfileId, application.ResumeId, application.AssignedRecruiterMemberId, application.CoverLetter, application.Status, application.SubmittedAtUtc);
    }
}
