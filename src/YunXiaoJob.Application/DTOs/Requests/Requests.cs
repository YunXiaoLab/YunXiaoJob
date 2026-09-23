using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.DTOs.Requests;

public record RegisterCandidateRequest(string Email, string Password, string FullName);
public record VerifyRegistrationOtpRequest(Guid ChallengeId, string Otp);
public record ResendRegistrationOtpRequest(Guid ChallengeId);
public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
public record RefreshAccessTokenRequest(string RefreshToken);
public record ResetPasswordRequest(string Token, string NewPassword);
public record UpdateCandidateProfileRequest(string? Headline, string? Summary, string? Location,
    int? YearsOfExperience, bool IsSearchable, string? AvatarUrl, DateOnly? DateOfBirth, Gender Gender,
    decimal? ExpectedMinSalary, decimal? ExpectedMaxSalary, string? ExpectedSalaryCurrency,
    string? LinkedInUrl, string? GitHubUrl, string? PortfolioUrl);
public record CandidateEducationRequest(string SchoolName, string? Major, EducationLevel Level, int? StartYear,
    int? EndYear, decimal? Gpa, decimal? GpaScale, bool IsCurrent, string? Description);
public record UpdateCandidateEducationsRequest(IReadOnlyList<CandidateEducationRequest> Educations);
public record CandidateExperienceRequest(string CompanyName, string JobTitle, EmploymentType? EmploymentType,
    string? Location, DateOnly StartDate, DateOnly? EndDate, bool IsCurrent, string? Description);
public record UpdateCandidateExperiencesRequest(IReadOnlyList<CandidateExperienceRequest> Experiences);
public record CandidateSkillRequest(string Name, SkillProficiency Proficiency, int? YearsOfExperience,
    int? LastUsedYear);
public record UpdateCandidateSkillsRequest(IReadOnlyList<CandidateSkillRequest> Skills);
public record AddResumeRequest(string Name, string FileUrl, bool IsDefault);
public record CreateCompanyRequest(string Name, string? Description, string? Website, string? LogoUrl,
    string? Address, string? Industry, int? EmployeeCount);
public record UpdateCompanyRequest(string Name, string? Description, string? Website, string? LogoUrl,
    string? Address, string? Industry, int? EmployeeCount);
public record AddCompanyMemberRequest(Guid UserId, CompanyMemberRole Role);
public record CreateJobPostingRequest(Guid CompanyId, string Title, string Description, string Requirements,
    string? Benefits, string Location, EmploymentType EmploymentType, WorkplaceType WorkplaceType,
    decimal? MinSalary, decimal? MaxSalary, string Currency, DateOnly? ApplicationDeadline,
    IReadOnlyList<string> Skills);
public record UpdateJobPostingRequest(string Title, string Description, string Requirements, string? Benefits,
    string Location, EmploymentType EmploymentType, WorkplaceType WorkplaceType, decimal? MinSalary,
    decimal? MaxSalary, string Currency, DateOnly? ApplicationDeadline, IReadOnlyList<string> Skills);
public record SearchJobPostingsRequest(string? Keyword, string? Location, EmploymentType? EmploymentType,
    WorkplaceType? WorkplaceType);
public record ApplyForJobRequest(Guid JobPostingId, Guid ResumeId, string? CoverLetter);
public record ChangeApplicationStatusRequest(ApplicationStatus Status, string? Note);
public record AssignRecruiterRequest(Guid RecruiterMemberId);
public record ScheduleInterviewRequest(DateTime StartsAtUtc, DateTime EndsAtUtc, string? LocationOrMeetingUrl,
    string? Note);
public record CompleteInterviewRequest(string? EvaluationNote, int? Rating);
public record CreateJobOfferRequest(decimal? Salary, string Currency, DateOnly? StartDate, DateOnly? ExpiresOn,
    string? Note);
public record RespondToJobOfferRequest(bool Accept);
public record SubmitCompanyRegistrationRequest(string CompanyName, string? Website, string? Address,
    string? Industry, string? Description, int? EmployeeCount, string ContactFullName, string ContactEmail,
    string? ContactPhoneNumber);
public record ReviewCompanyRegistrationRequest(string? Note);
public record CreateCompanyStaffAccountRequest(string Email, string FullName, string? PhoneNumber,
    CompanyMemberRole Role);
