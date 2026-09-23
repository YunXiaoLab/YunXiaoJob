using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.DTOs.Responses;

public record UserResponse(Guid Id, string Email, string FullName, string? PhoneNumber, PlatformRole PlatformRole, bool IsActive);
public record AuthenticationResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAtUtc, UserResponse User);
public record RegistrationChallengeResponse(Guid ChallengeId, DateTime ExpiresAtUtc, DateTime ResendAvailableAtUtc);
public record CandidateProfileResponse(Guid Id, Guid UserId, string? Headline, string? Summary, string? Location,
    int? YearsOfExperience, bool IsSearchable, string? AvatarUrl, DateOnly? DateOfBirth, Gender Gender,
    decimal? ExpectedMinSalary, decimal? ExpectedMaxSalary, string? ExpectedSalaryCurrency, string? LinkedInUrl,
    string? GitHubUrl, string? PortfolioUrl, IReadOnlyList<ResumeResponse> Resumes,
    IReadOnlyList<CandidateEducationResponse> Educations, IReadOnlyList<CandidateExperienceResponse> Experiences,
    IReadOnlyList<CandidateSkillResponse> Skills);
public record CandidateEducationResponse(Guid Id, string SchoolName, string? Major, EducationLevel Level,
    int? StartYear, int? EndYear, decimal? Gpa, decimal? GpaScale, bool IsCurrent, string? Description);
public record CandidateExperienceResponse(Guid Id, string CompanyName, string JobTitle,
    EmploymentType? EmploymentType, string? Location, DateOnly StartDate, DateOnly? EndDate, bool IsCurrent,
    string? Description, int TotalMonths);
public record CandidateSkillResponse(Guid Id, string Name, SkillProficiency Proficiency, int? YearsOfExperience,
    int? LastUsedYear);
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
public record PlatformDashboardResponse(int Users, int Candidates, int Companies, int ActiveCompanies, int JobPostings, int PublishedJobPostings, int Applications);
public record CompanyDashboardResponse(int JobPostings, int PublishedJobPostings, int Applications, IReadOnlyDictionary<ApplicationStatus, int> ApplicationsByStatus);
public record CompanyRegistrationResponse(Guid Id, string CompanyName, string? Website, string? Address,
    string? Industry, string? Description, int? EmployeeCount, string ContactFullName, string ContactEmail,
    string? ContactPhoneNumber, CompanyRegistrationStatus Status, string? ReviewNote, DateTime? ReviewedAtUtc,
    Guid? CreatedCompanyId, Guid? CreatedOwnerUserId, DateTime CreatedAtUtc);
public record CompanyStaffAccountResponse(Guid MemberId, Guid CompanyId, Guid UserId, string Email,
    string FullName, CompanyMemberRole Role, bool IsActive);
public record JobPostingSummaryResponse(Guid Id, Guid CompanyId, string CompanyName, string? CompanyLogoUrl,
    string Title, string Location, EmploymentType EmploymentType, WorkplaceType WorkplaceType, decimal? MinSalary,
    decimal? MaxSalary, string Currency, DateOnly? ApplicationDeadline, JobPostingStatus Status,
    DateTime? PublishedAtUtc, IReadOnlyList<string> Skills);
public record JobPostingDetailResponse(JobPostingResponse Job, CompanyResponse Company);
public record CompanyMembershipResponse(CompanyResponse Company, Guid MemberId, CompanyMemberRole Role, bool IsActive);
public record CompanyMemberDetailResponse(Guid Id, Guid CompanyId, Guid UserId, string Email, string FullName,
    string? PhoneNumber, CompanyMemberRole Role, bool IsActive);
public record CandidateSummaryResponse(Guid Id, Guid UserId, string FullName, string Email, string? PhoneNumber,
    string? Headline, string? Summary, string? Location, int? YearsOfExperience, string? AvatarUrl,
    string? LinkedInUrl, string? GitHubUrl, string? PortfolioUrl, IReadOnlyList<CandidateSkillResponse> Skills,
    ResumeResponse? Resume);
public record ApplicationStatusHistoryResponse(Guid Id, ApplicationStatus? FromStatus, ApplicationStatus ToStatus,
    string? Note, DateTime CreatedAtUtc);
public record JobApplicationListItemResponse(JobApplicationResponse Application, JobPostingSummaryResponse? Job,
    CandidateSummaryResponse? Candidate);
public record JobApplicationDetailResponse(JobApplicationResponse Application, JobPostingSummaryResponse? Job,
    CandidateSummaryResponse? Candidate, IReadOnlyList<InterviewResponse> Interviews,
    IReadOnlyList<JobOfferResponse> Offers, IReadOnlyList<ApplicationStatusHistoryResponse> History);
