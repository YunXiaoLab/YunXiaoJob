using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;
using YunXiaoJob.Infrastructure.Data;

namespace YunXiaoJob.Infrastructure.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly ApplicationDbContext _context;
    public JobApplicationRepository(ApplicationDbContext context) => _context = context;

    public Task<JobApplication?> GetByIdAsync(Guid applicationId, CancellationToken cancellationToken = default) =>
        _context.JobApplications.FirstOrDefaultAsync(x => x.Id == applicationId, cancellationToken);

    public Task<JobApplication?> GetDetailByIdAsync(Guid applicationId,
        CancellationToken cancellationToken = default) =>
        _context.JobApplications
            .Include(x => x.StatusHistory)
            .Include(x => x.Interviews)
            .Include(x => x.Offers)
            .FirstOrDefaultAsync(x => x.Id == applicationId, cancellationToken);

    public Task<Interview?> GetInterviewByIdAsync(Guid interviewId, CancellationToken cancellationToken = default) =>
        _context.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId, cancellationToken);

    public Task<JobOffer?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default) =>
        _context.JobOffers.FirstOrDefaultAsync(x => x.Id == offerId, cancellationToken);

    public Task<JobApplication?> GetByJobPostingAndCandidateAsync(Guid jobPostingId, Guid candidateProfileId,
        CancellationToken cancellationToken = default) =>
        _context.JobApplications.FirstOrDefaultAsync(x => x.JobPostingId == jobPostingId &&
            x.CandidateProfileId == candidateProfileId, cancellationToken);

    public async Task<IReadOnlyList<JobApplication>> GetByJobPostingAsync(Guid jobPostingId,
        ApplicationStatus? status, CancellationToken cancellationToken = default)
    {
        IQueryable<JobApplication> query = _context.JobApplications.Where(x => x.JobPostingId == jobPostingId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return await query.OrderByDescending(x => x.SubmittedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JobApplication>> GetByCandidateAsync(Guid candidateProfileId,
        CancellationToken cancellationToken = default) =>
        await _context.JobApplications
            .Where(x => x.CandidateProfileId == candidateProfileId)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public Task AddAsync(JobApplication application, CancellationToken cancellationToken = default) =>
        _context.JobApplications.AddAsync(application, cancellationToken).AsTask();

    public Task AddStatusHistoryAsync(ApplicationStatusHistory history,
        CancellationToken cancellationToken = default) =>
        _context.ApplicationStatusHistories.AddAsync(history, cancellationToken).AsTask();

    public Task AddInterviewAsync(Interview interview, CancellationToken cancellationToken = default) =>
        _context.Interviews.AddAsync(interview, cancellationToken).AsTask();

    public Task AddOfferAsync(JobOffer offer, CancellationToken cancellationToken = default) =>
        _context.JobOffers.AddAsync(offer, cancellationToken).AsTask();
}
