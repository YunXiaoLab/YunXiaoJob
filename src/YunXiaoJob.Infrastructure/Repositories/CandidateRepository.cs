using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Infrastructure.Data;

namespace YunXiaoJob.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly ApplicationDbContext _context;
    public CandidateRepository(ApplicationDbContext context) => _context = context;

    public Task<CandidateProfile?> GetProfileByIdAsync(Guid candidateProfileId, bool includeResumes = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<CandidateProfile> query = _context.CandidateProfiles;
        if (includeResumes) query = query.Include(x => x.Resumes);
        return query.FirstOrDefaultAsync(x => x.Id == candidateProfileId, cancellationToken);
    }

    public Task<CandidateProfile?> GetProfileByUserIdAsync(Guid userId, bool includeResumes = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<CandidateProfile> query = _context.CandidateProfiles;
        if (includeResumes) query = query.Include(x => x.Resumes);
        return query.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<Resume?> GetResumeByIdAsync(Guid resumeId, CancellationToken cancellationToken = default) =>
        _context.Resumes.FirstOrDefaultAsync(x => x.Id == resumeId, cancellationToken);

    public Task<SavedJob?> GetSavedJobAsync(Guid candidateProfileId, Guid jobPostingId,
        CancellationToken cancellationToken = default) =>
        _context.SavedJobs.FirstOrDefaultAsync(x => x.CandidateProfileId == candidateProfileId &&
            x.JobPostingId == jobPostingId, cancellationToken);

    public Task AddProfileAsync(CandidateProfile profile, CancellationToken cancellationToken = default) =>
        _context.CandidateProfiles.AddAsync(profile, cancellationToken).AsTask();

    public Task AddResumeAsync(Resume resume, CancellationToken cancellationToken = default) =>
        _context.Resumes.AddAsync(resume, cancellationToken).AsTask();

    public Task AddSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default) =>
        _context.SavedJobs.AddAsync(savedJob, cancellationToken).AsTask();

    public void RemoveSavedJob(SavedJob savedJob) => _context.SavedJobs.Remove(savedJob);
}
