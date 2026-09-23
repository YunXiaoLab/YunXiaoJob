using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.Candidates;

public class UpdateCandidateEducationsUseCase
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCandidateEducationsUseCase(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    { _candidates = candidates; _unitOfWork = unitOfWork; }

    public async Task<IReadOnlyList<CandidateEducationResponse>> ExecuteAsync(UpdateCandidateEducationsRequest request,
        Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var items = request.Educations ?? [];
        var educations = new List<CandidateEducation>(items.Count);
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.SchoolName))
                throw new InvalidOperationException("School name is required.");
            if (item.StartYear is not null && item.EndYear is not null && item.StartYear > item.EndYear)
                throw new InvalidOperationException("Education start year cannot be after the end year.");
            if (item.Gpa is not null && (item.Gpa < 0 || item.GpaScale is not null && item.Gpa > item.GpaScale))
                throw new InvalidOperationException("GPA must be between 0 and the GPA scale.");
            educations.Add(new CandidateEducation
            {
                CandidateProfileId = profile.Id, SchoolName = item.SchoolName.Trim(), Major = item.Major?.Trim(),
                Level = item.Level, StartYear = item.StartYear, EndYear = item.IsCurrent ? null : item.EndYear,
                Gpa = item.Gpa, GpaScale = item.GpaScale, IsCurrent = item.IsCurrent,
                Description = item.Description?.Trim()
            });
        }
        _candidates.RemoveEducations(profile.Educations.ToList());
        await _candidates.AddEducationsAsync(educations, cancellationToken);
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return educations.Select(x => x.ToResponse()).ToList();
    }
}

public class UpdateCandidateExperiencesUseCase
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCandidateExperiencesUseCase(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    { _candidates = candidates; _unitOfWork = unitOfWork; }

    public async Task<IReadOnlyList<CandidateExperienceResponse>> ExecuteAsync(
        UpdateCandidateExperiencesRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var items = request.Experiences ?? [];
        var experiences = new List<CandidateExperience>(items.Count);
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.CompanyName))
                throw new InvalidOperationException("Company name is required.");
            if (string.IsNullOrWhiteSpace(item.JobTitle))
                throw new InvalidOperationException("Job title is required.");
            if (item.StartDate > today)
                throw new InvalidOperationException("Experience start date cannot be in the future.");
            var endDate = item.IsCurrent ? null : item.EndDate;
            if (endDate is not null && endDate < item.StartDate)
                throw new InvalidOperationException("Experience end date cannot be before the start date.");
            experiences.Add(new CandidateExperience
            {
                CandidateProfileId = profile.Id, CompanyName = item.CompanyName.Trim(),
                JobTitle = item.JobTitle.Trim(), EmploymentType = item.EmploymentType,
                Location = item.Location?.Trim(), StartDate = item.StartDate, EndDate = endDate,
                IsCurrent = item.IsCurrent, Description = item.Description?.Trim()
            });
        }
        _candidates.RemoveExperiences(profile.Experiences.ToList());
        await _candidates.AddExperiencesAsync(experiences, cancellationToken);
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return experiences.Select(x => x.ToResponse()).ToList();
    }
}

public class UpdateCandidateSkillsUseCase
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCandidateSkillsUseCase(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    { _candidates = candidates; _unitOfWork = unitOfWork; }

    public async Task<IReadOnlyList<CandidateSkillResponse>> ExecuteAsync(UpdateCandidateSkillsRequest request,
        Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var currentYear = DateTime.UtcNow.Year;
        var items = request.Skills ?? [];
        var skills = new List<CandidateSkill>(items.Count);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new InvalidOperationException("Skill name is required.");
            var name = item.Name.Trim();
            if (!seen.Add(name)) throw new InvalidOperationException($"Skill \"{name}\" is duplicated.");
            if (item.YearsOfExperience is < 0)
                throw new InvalidOperationException("Skill years of experience cannot be negative.");
            if (item.LastUsedYear is not null && item.LastUsedYear > currentYear)
                throw new InvalidOperationException("Skill last used year cannot be in the future.");
            skills.Add(new CandidateSkill
            {
                CandidateProfileId = profile.Id, Name = name, Proficiency = item.Proficiency,
                YearsOfExperience = item.YearsOfExperience, LastUsedYear = item.LastUsedYear
            });
        }
        _candidates.RemoveSkills(profile.Skills.ToList());
        await _candidates.AddSkillsAsync(skills, cancellationToken);
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return skills.Select(x => x.ToResponse()).ToList();
    }
}
