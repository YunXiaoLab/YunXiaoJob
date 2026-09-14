using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.Interfaces.Repositories;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<JobApplication?> GetDetailByIdAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<Interview?> GetInterviewByIdAsync(Guid interviewId, CancellationToken cancellationToken = default);
    Task<JobOffer?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<JobApplication?> GetByJobPostingAndCandidateAsync(Guid jobPostingId, Guid candidateProfileId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobApplication>> GetByJobPostingAsync(Guid jobPostingId, ApplicationStatus? status,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobApplication>> GetByCandidateAsync(Guid candidateProfileId,
        CancellationToken cancellationToken = default);
    Task AddAsync(JobApplication application, CancellationToken cancellationToken = default);
    Task AddStatusHistoryAsync(ApplicationStatusHistory history, CancellationToken cancellationToken = default);
    Task AddInterviewAsync(Interview interview, CancellationToken cancellationToken = default);
    Task AddOfferAsync(JobOffer offer, CancellationToken cancellationToken = default);
}
