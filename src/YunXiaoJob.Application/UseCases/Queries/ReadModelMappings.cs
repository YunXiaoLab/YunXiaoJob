using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.Queries;

/// <summary>Entity to read-model projections shared by the query use cases.</summary>
public static class ReadModelMappings
{
    public static CompanyResponse ToResponse(this Company company) => new(company.Id, company.Name, company.LogoUrl,
        company.Description, company.Website, company.Address, company.Industry, company.EmployeeCount,
        company.Status);

    public static JobPostingResponse ToResponse(this JobPosting job) => new(job.Id, job.CompanyId,
        job.CreatedByMemberId, job.AssignedRecruiterMemberId, job.Title, job.Description, job.Requirements,
        job.Benefits, job.Location, job.EmploymentType, job.WorkplaceType, job.MinSalary, job.MaxSalary, job.Currency,
        job.ApplicationDeadline, job.Status, job.PublishedAtUtc, job.Skills.Select(x => x.Name).ToList());

    public static JobPostingSummaryResponse ToSummary(this JobPosting job, Company? company) => new(job.Id,
        job.CompanyId, company?.Name ?? string.Empty, company?.LogoUrl, job.Title, job.Location, job.EmploymentType,
        job.WorkplaceType, job.MinSalary, job.MaxSalary, job.Currency, job.ApplicationDeadline, job.Status,
        job.PublishedAtUtc, job.Skills.Select(x => x.Name).ToList());

    public static JobApplicationResponse ToResponse(this JobApplication application) => new(application.Id,
        application.JobPostingId, application.CandidateProfileId, application.ResumeId,
        application.AssignedRecruiterMemberId, application.CoverLetter, application.Status,
        application.SubmittedAtUtc);

    public static InterviewResponse ToResponse(this Interview interview) => new(interview.Id,
        interview.JobApplicationId, interview.StartsAtUtc, interview.EndsAtUtc, interview.LocationOrMeetingUrl,
        interview.Status, interview.EvaluationNote, interview.Rating);

    public static JobOfferResponse ToResponse(this JobOffer offer) => new(offer.Id, offer.JobApplicationId,
        offer.Salary, offer.Currency, offer.StartDate, offer.ExpiresOn, offer.Note, offer.Status);

    public static ApplicationStatusHistoryResponse ToResponse(this ApplicationStatusHistory history) =>
        new(history.Id, history.FromStatus, history.ToStatus, history.Note, history.CreatedAtUtc);

    public static ResumeResponse ToResponse(this Resume resume) =>
        new(resume.Id, resume.Name, resume.FileUrl, resume.IsDefault);

    public static CandidateSummaryResponse ToSummary(this CandidateProfile profile, User? user, Resume? resume) =>
        new(profile.Id, profile.UserId, user?.FullName ?? string.Empty, user?.Email ?? string.Empty,
            user?.PhoneNumber, profile.Headline, profile.Summary, profile.Location, profile.YearsOfExperience,
            profile.AvatarUrl, profile.LinkedInUrl, profile.GitHubUrl, profile.PortfolioUrl,
            profile.Skills.Select(x => new CandidateSkillResponse(x.Id, x.Name, x.Proficiency, x.YearsOfExperience,
                x.LastUsedYear)).ToList(),
            resume?.ToResponse());
}
