using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class ScheduleInterviewUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly ICandidateRepository _candidates; private readonly IUserRepository _users; private readonly INotificationRepository _notifications; private readonly IMeetingService _meetings; private readonly IUnitOfWork _unitOfWork;
    public ScheduleInterviewUseCase(IJobApplicationRepository applications, IJobPostingRepository jobs, ICompanyRepository companies, ICandidateRepository candidates, IUserRepository users, INotificationRepository notifications, IMeetingService meetings, IUnitOfWork unitOfWork) { _applications = applications; _jobs = jobs; _companies = companies; _candidates = candidates; _users = users; _notifications = notifications; _meetings = meetings; _unitOfWork = unitOfWork; }
    public async Task<InterviewResponse> ExecuteAsync(Guid applicationId, ScheduleInterviewRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        if (request.EndsAtUtc <= request.StartsAtUtc) throw new InvalidOperationException("Interview end time must be after start time.");
        var application = await _applications.GetByIdAsync(applicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        var job = await _jobs.GetByIdAsync(application.JobPostingId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        var permitted = member is not null && member.IsActive && (member.Role is CompanyMemberRole.Owner or CompanyMemberRole.HR || application.AssignedRecruiterMemberId == member.Id);
        if (!permitted) throw new UnauthorizedAccessException();
        if (application.Status is not (ApplicationStatus.Shortlisted or ApplicationStatus.Interviewing)) throw new InvalidOperationException("Only shortlisted or interviewing applications can be scheduled.");
        var profile = await _candidates.GetProfileByIdAsync(application.CandidateProfileId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var candidate = await _users.GetByIdAsync(profile.UserId, cancellationToken) ?? throw new KeyNotFoundException("Candidate user was not found.");
        var meetingUrl = await _meetings.CreateGoogleMeetAsync($"Interview: {job.Title}", candidate.Email, request.StartsAtUtc, request.EndsAtUtc, cancellationToken);
        var interview = new Interview { JobApplicationId = application.Id, ScheduledByMemberId = member!.Id, StartsAtUtc = request.StartsAtUtc, EndsAtUtc = request.EndsAtUtc, LocationOrMeetingUrl = meetingUrl, Note = request.Note?.Trim(), Status = InterviewStatus.Scheduled };
        if (application.Status != ApplicationStatus.Interviewing) { var previous = application.Status; application.Status = ApplicationStatus.Interviewing; await _applications.AddStatusHistoryAsync(new ApplicationStatusHistory { JobApplicationId = application.Id, FromStatus = previous, ToStatus = application.Status, ChangedByMemberId = member.Id, Note = "Interview scheduled." }, cancellationToken); }
        await _applications.AddInterviewAsync(interview, cancellationToken);
        await _notifications.AddAsync(new Notification { UserId = candidate.Id, Type = NotificationType.InterviewScheduled, Title = "Interview scheduled", Content = $"Your interview for {job.Title} is scheduled for {request.StartsAtUtc:u}. Join Google Meet: {meetingUrl}", TargetUrl = $"/candidate/applications" }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new InterviewResponse(interview.Id, interview.JobApplicationId, interview.StartsAtUtc, interview.EndsAtUtc, interview.LocationOrMeetingUrl, interview.Status, interview.EvaluationNote, interview.Rating);
    }
}
