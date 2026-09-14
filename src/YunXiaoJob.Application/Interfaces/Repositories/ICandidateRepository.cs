using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.Interfaces.Repositories;

public interface ICandidateRepository
{
    Task<CandidateProfile?> GetProfileByIdAsync(Guid candidateProfileId, bool includeResumes = false,
        CancellationToken cancellationToken = default);
    Task<CandidateProfile?> GetProfileByUserIdAsync(Guid userId, bool includeResumes = false,
        CancellationToken cancellationToken = default);
    Task<Resume?> GetResumeByIdAsync(Guid resumeId, CancellationToken cancellationToken = default);
    Task<SavedJob?> GetSavedJobAsync(Guid candidateProfileId, Guid jobPostingId,
        CancellationToken cancellationToken = default);
    Task AddProfileAsync(CandidateProfile profile, CancellationToken cancellationToken = default);
    Task AddResumeAsync(Resume resume, CancellationToken cancellationToken = default);
    Task AddSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default);
    void RemoveSavedJob(SavedJob savedJob);
}
