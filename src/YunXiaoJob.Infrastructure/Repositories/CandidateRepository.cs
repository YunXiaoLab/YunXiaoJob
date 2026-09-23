using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Infrastructure.Data;

namespace YunXiaoJob.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly ApplicationDbContext _context;
    public CandidateRepository(ApplicationDbContext context) => _context = context;

    public Task<CandidateProfile?> GetProfileByIdAsync(Guid candidateProfileId, bool includeDetails = false,
        CancellationToken cancellationToken = default) =>
        Query(includeDetails).FirstOrDefaultAsync(x => x.Id == candidateProfileId, cancellationToken);

    public Task<CandidateProfile?> GetProfileByUserIdAsync(Guid userId, bool includeDetails = false,
        CancellationToken cancellationToken = default) =>
        Query(includeDetails).FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    private IQueryable<CandidateProfile> Query(bool includeDetails)
    {
        IQueryable<CandidateProfile> query = _context.CandidateProfiles;
        if (!includeDetails) return query;
        return query.Include(x => x.Resumes).Include(x => x.Educations).Include(x => x.Experiences)
            .Include(x => x.Skills);
    }

    public async Task<IReadOnlyList<CandidateProfile>> GetProfilesByIdsAsync(
        IReadOnlyCollection<Guid> candidateProfileIds, bool includeDetails = false,
        CancellationToken cancellationToken = default) =>
        candidateProfileIds.Count == 0
            ? []
            : await Query(includeDetails).Where(x => candidateProfileIds.Contains(x.Id)).ToListAsync(cancellationToken);

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

    public Task AddEducationsAsync(IEnumerable<CandidateEducation> educations,
        CancellationToken cancellationToken = default) =>
        _context.CandidateEducations.AddRangeAsync(educations, cancellationToken);

    public void RemoveEducations(IEnumerable<CandidateEducation> educations) =>
        _context.CandidateEducations.RemoveRange(educations);

    public Task AddExperiencesAsync(IEnumerable<CandidateExperience> experiences,
        CancellationToken cancellationToken = default) =>
        _context.CandidateExperiences.AddRangeAsync(experiences, cancellationToken);

    public void RemoveExperiences(IEnumerable<CandidateExperience> experiences) =>
        _context.CandidateExperiences.RemoveRange(experiences);

    public Task AddSkillsAsync(IEnumerable<CandidateSkill> skills, CancellationToken cancellationToken = default) =>
        _context.CandidateSkills.AddRangeAsync(skills, cancellationToken);

    public void RemoveSkills(IEnumerable<CandidateSkill> skills) => _context.CandidateSkills.RemoveRange(skills);
}
