using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class CompleteInterviewUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unitOfWork;
    public CompleteInterviewUseCase(IJobApplicationRepository applications, IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unitOfWork) { _applications = applications; _jobs = jobs; _companies = companies; _unitOfWork = unitOfWork; }
    public async Task<InterviewResponse> ExecuteAsync(Guid interviewId, CompleteInterviewRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        if (request.Rating is < 1 or > 5) throw new InvalidOperationException("Rating must be between 1 and 5.");
        var interview = await _applications.GetInterviewByIdAsync(interviewId, cancellationToken) ?? throw new KeyNotFoundException("Interview was not found.");
        var application = await _applications.GetByIdAsync(interview.JobApplicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        var job = await _jobs.GetByIdAsync(application.JobPostingId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive || (member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR) && application.AssignedRecruiterMemberId != member.Id)) throw new UnauthorizedAccessException();
        interview.Status = InterviewStatus.Completed; interview.EvaluationNote = request.EvaluationNote?.Trim(); interview.Rating = request.Rating; interview.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new InterviewResponse(interview.Id, interview.JobApplicationId, interview.StartsAtUtc, interview.EndsAtUtc, interview.LocationOrMeetingUrl, interview.Status, interview.EvaluationNote, interview.Rating);
    }
}
