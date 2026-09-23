using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.Candidates;

internal static class CandidateProfileMapping
{
    public static CandidateProfileResponse ToResponse(this CandidateProfile x) =>
        new(x.Id, x.UserId, x.Headline, x.Summary, x.Location, x.YearsOfExperience, x.IsSearchable, x.AvatarUrl,
            x.DateOfBirth, x.Gender, x.ExpectedMinSalary, x.ExpectedMaxSalary, x.ExpectedSalaryCurrency,
            x.LinkedInUrl, x.GitHubUrl, x.PortfolioUrl,
            x.Resumes.Select(r => new ResumeResponse(r.Id, r.Name, r.FileUrl, r.IsDefault)).ToList(),
            x.Educations.OrderByDescending(e => e.EndYear ?? int.MaxValue).ThenByDescending(e => e.StartYear)
                .Select(ToResponse).ToList(),
            x.Experiences.OrderByDescending(e => e.StartDate).Select(ToResponse).ToList(),
            x.Skills.OrderBy(s => s.Name).Select(ToResponse).ToList());

    public static CandidateEducationResponse ToResponse(this CandidateEducation x) =>
        new(x.Id, x.SchoolName, x.Major, x.Level, x.StartYear, x.EndYear, x.Gpa, x.GpaScale, x.IsCurrent,
            x.Description);

    public static CandidateExperienceResponse ToResponse(this CandidateExperience x) =>
        new(x.Id, x.CompanyName, x.JobTitle, x.EmploymentType, x.Location, x.StartDate, x.EndDate, x.IsCurrent,
            x.Description, TotalMonths(x));

    public static CandidateSkillResponse ToResponse(this CandidateSkill x) =>
        new(x.Id, x.Name, x.Proficiency, x.YearsOfExperience, x.LastUsedYear);

    private static int TotalMonths(CandidateExperience x)
    {
        var end = x.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        if (end < x.StartDate) return 0;
        return ((end.Year - x.StartDate.Year) * 12) + end.Month - x.StartDate.Month;
    }
}
