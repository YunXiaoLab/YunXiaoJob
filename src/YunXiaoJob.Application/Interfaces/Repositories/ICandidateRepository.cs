using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.Interfaces.Repositories;

public interface ICandidateRepository
{
    Task<CandidateProfile?> GetProfileByIdAsync(Guid candidateProfileId, bool includeDetails = false,
        CancellationToken cancellationToken = default);
    Task<CandidateProfile?> GetProfileByUserIdAsync(Guid userId, bool includeDetails = false,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CandidateProfile>> GetProfilesByIdsAsync(IReadOnlyCollection<Guid> candidateProfileIds,
        bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<Resume?> GetResumeByIdAsync(Guid resumeId, CancellationToken cancellationToken = default);
    Task<SavedJob?> GetSavedJobAsync(Guid candidateProfileId, Guid jobPostingId,
        CancellationToken cancellationToken = default);
    Task AddProfileAsync(CandidateProfile profile, CancellationToken cancellationToken = default);
    Task AddResumeAsync(Resume resume, CancellationToken cancellationToken = default);
    Task AddSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default);
    void RemoveSavedJob(SavedJob savedJob);
    Task AddEducationsAsync(IEnumerable<CandidateEducation> educations, CancellationToken cancellationToken = default);
    void RemoveEducations(IEnumerable<CandidateEducation> educations);
    Task AddExperiencesAsync(IEnumerable<CandidateExperience> experiences, CancellationToken cancellationToken = default);
    void RemoveExperiences(IEnumerable<CandidateExperience> experiences);
    Task AddSkillsAsync(IEnumerable<CandidateSkill> skills, CancellationToken cancellationToken = default);
    void RemoveSkills(IEnumerable<CandidateSkill> skills);
}
