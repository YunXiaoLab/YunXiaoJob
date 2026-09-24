using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class ApplyForJobUseCase
{
    private readonly ICandidateRepository _candidates; private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IJobApplicationRepository _applications; private readonly IUnitOfWork _unitOfWork;
    public ApplyForJobUseCase(ICandidateRepository candidates, IJobPostingRepository jobs, ICompanyRepository companies, IJobApplicationRepository applications, IUnitOfWork unitOfWork) { _candidates = candidates; _jobs = jobs; _companies = companies; _applications = applications; _unitOfWork = unitOfWork; }
    public async Task<JobApplicationResponse> ExecuteAsync(ApplyForJobRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken) ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var resume = profile.Resumes.FirstOrDefault(x => x.Id == request.ResumeId) ?? throw new UnauthorizedAccessException("Resume does not belong to the current candidate.");
        var job = await _jobs.GetByIdAsync(request.JobPostingId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        if (job.Status != JobPostingStatus.Published || (job.ApplicationDeadline.HasValue && job.ApplicationDeadline < DateOnly.FromDateTime(DateTime.UtcNow))) throw new InvalidOperationException("Job posting is not open for applications.");
        var company = await _companies.GetByIdAsync(job.CompanyId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Company was not found.");
        if (company.Status != CompanyStatus.Active) throw new InvalidOperationException("Job posting is not open for applications.");
        if (await _applications.GetByJobPostingAndCandidateAsync(job.Id, profile.Id, cancellationToken) is not null) throw new InvalidOperationException("Candidate has already applied for this job posting.");
        var application = new JobApplication { JobPostingId = job.Id, CandidateProfileId = profile.Id, ResumeId = resume.Id, CoverLetter = request.CoverLetter?.Trim(), Status = ApplicationStatus.Submitted };
        var history = new ApplicationStatusHistory { JobApplicationId = application.Id, ToStatus = ApplicationStatus.Submitted };
        await _applications.AddAsync(application, cancellationToken); await _applications.AddStatusHistoryAsync(history, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobApplicationResponse(application.Id, application.JobPostingId, application.CandidateProfileId, application.ResumeId, application.AssignedRecruiterMemberId, application.CoverLetter, application.Status, application.SubmittedAtUtc);
    }
}
