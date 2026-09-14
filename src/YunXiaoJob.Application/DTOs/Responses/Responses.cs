using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.DTOs.Responses;

public record UserResponse(Guid Id, string Email, string FullName, string? PhoneNumber, PlatformRole PlatformRole, bool IsActive);
public record AuthenticationResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAtUtc, UserResponse User);
public record RegistrationChallengeResponse(Guid ChallengeId, DateTime ExpiresAtUtc, DateTime ResendAvailableAtUtc);
public record CandidateProfileResponse(Guid Id, Guid UserId, string? Headline, string? Summary, string? Location,
    int? YearsOfExperience, bool IsSearchable, IReadOnlyList<ResumeResponse> Resumes);
public record ResumeResponse(Guid Id, string Name, string FileUrl, bool IsDefault);
public record CompanyResponse(Guid Id, string Name, string? LogoUrl, string? Description, string? Website,
    string? Address, string? Industry, int? EmployeeCount, CompanyStatus Status);
public record CompanyMemberResponse(Guid Id, Guid CompanyId, Guid UserId, CompanyMemberRole Role, bool IsActive);
public record JobPostingResponse(Guid Id, Guid CompanyId, Guid CreatedByMemberId, Guid? AssignedRecruiterMemberId,
    string Title, string Description, string Requirements, string? Benefits, string Location,
    EmploymentType EmploymentType, WorkplaceType WorkplaceType, decimal? MinSalary, decimal? MaxSalary,
    string Currency, DateOnly? ApplicationDeadline, JobPostingStatus Status, DateTime? PublishedAtUtc,
    IReadOnlyList<string> Skills);
public record JobApplicationResponse(Guid Id, Guid JobPostingId, Guid CandidateProfileId, Guid ResumeId,
    Guid? AssignedRecruiterMemberId, string? CoverLetter, ApplicationStatus Status, DateTime SubmittedAtUtc);
public record InterviewResponse(Guid Id, Guid JobApplicationId, DateTime StartsAtUtc, DateTime EndsAtUtc,
    string? LocationOrMeetingUrl, InterviewStatus Status, string? EvaluationNote, int? Rating);
public record JobOfferResponse(Guid Id, Guid JobApplicationId, decimal? Salary, string Currency,
    DateOnly? StartDate, DateOnly? ExpiresOn, string? Note, OfferStatus Status);
public record NotificationResponse(Guid Id, string Title, string Content, NotificationType Type, string? TargetUrl, bool IsRead, DateTime CreatedAtUtc);
public record CatalogItemResponse(Guid Id, string Name);
public record PlatformDashboardResponse(int Users, int Candidates, int Companies, int ActiveCompanies, int JobPostings, int PublishedJobPostings, int Applications);
public record CompanyDashboardResponse(int JobPostings, int PublishedJobPostings, int Applications, IReadOnlyDictionary<ApplicationStatus, int> ApplicationsByStatus);
