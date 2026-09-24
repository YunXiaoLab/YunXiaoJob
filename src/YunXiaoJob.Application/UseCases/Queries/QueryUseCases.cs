using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Queries;

public class GetCompanyJobPostingsUseCase
{
    private readonly ICompanyRepository _companies; private readonly IJobPostingRepository _jobs;
    public GetCompanyJobPostingsUseCase(ICompanyRepository companies, IJobPostingRepository jobs)
    { _companies = companies; _jobs = jobs; }

    public async Task<IReadOnlyList<JobPostingResponse>> ExecuteAsync(Guid companyId, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var member = await _companies.GetMemberAsync(companyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive) throw new UnauthorizedAccessException();
        return (await _jobs.GetByCompanyAsync(companyId, cancellationToken)).Select(x => x.ToResponse()).ToList();
    }
}

public class GetJobApplicationsUseCase
{
    private readonly ICompanyRepository _companies; private readonly IJobPostingRepository _jobs;
    private readonly IJobApplicationRepository _applications; private readonly ICandidateRepository _candidates;
    private readonly IUserRepository _users;
    public GetJobApplicationsUseCase(ICompanyRepository companies, IJobPostingRepository jobs,
        IJobApplicationRepository applications, ICandidateRepository candidates, IUserRepository users)
    { _companies = companies; _jobs = jobs; _applications = applications; _candidates = candidates; _users = users; }

    public async Task<IReadOnlyList<JobApplicationListItemResponse>> ExecuteAsync(Guid jobPostingId,
        ApplicationStatus? status, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var job = await _jobs.GetByIdAsync(jobPostingId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive) throw new UnauthorizedAccessException();
        var values = await _applications.GetByJobPostingAsync(jobPostingId, status, cancellationToken);
        if (member.Role == CompanyMemberRole.Recruiter)
            values = values.Where(x => x.AssignedRecruiterMemberId == member.Id).ToList();

        var company = await _companies.GetByIdAsync(job.CompanyId, cancellationToken: cancellationToken);
        var summary = job.ToSummary(company);
        var profiles = await _candidates.GetProfilesByIdsAsync(
            values.Select(x => x.CandidateProfileId).Distinct().ToList(), true, cancellationToken);
        var users = (await _users.GetByIdsAsync(profiles.Select(x => x.UserId).Distinct().ToList(),
            cancellationToken)).ToDictionary(x => x.Id);
        var profilesById = profiles.ToDictionary(x => x.Id);

        return values.Select(application =>
        {
            profilesById.TryGetValue(application.CandidateProfileId, out var profile);
            CandidateSummaryResponse? candidate = null;
            if (profile is not null)
            {
                users.TryGetValue(profile.UserId, out var user);
                candidate = profile.ToSummary(user, profile.Resumes.FirstOrDefault(x => x.Id == application.ResumeId));
            }
            return new JobApplicationListItemResponse(application.ToResponse(), summary, candidate);
        }).ToList();
    }
}

public class GetMyApplicationsUseCase
{
    private readonly ICandidateRepository _candidates; private readonly IJobApplicationRepository _applications;
    private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies;
    public GetMyApplicationsUseCase(ICandidateRepository candidates, IJobApplicationRepository applications,
        IJobPostingRepository jobs, ICompanyRepository companies)
    { _candidates = candidates; _applications = applications; _jobs = jobs; _companies = companies; }

    public async Task<IReadOnlyList<JobApplicationListItemResponse>> ExecuteAsync(Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var values = await _applications.GetByCandidateAsync(profile.Id, cancellationToken);
        var jobs = await _jobs.GetByIdsAsync(values.Select(x => x.JobPostingId).Distinct().ToList(),
            cancellationToken);
        var companies = (await _companies.GetByIdsAsync(jobs.Select(x => x.CompanyId).Distinct().ToList(),
            cancellationToken)).ToDictionary(x => x.Id);
        var jobsById = jobs.ToDictionary(x => x.Id);

        return values.Select(application =>
        {
            JobPostingSummaryResponse? summary = null;
            if (jobsById.TryGetValue(application.JobPostingId, out var job))
                summary = job.ToSummary(companies.GetValueOrDefault(job.CompanyId));
            return new JobApplicationListItemResponse(application.ToResponse(), summary, null);
        }).ToList();
    }
}

public class GetJobPostingDetailUseCase
{
    private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies;
    public GetJobPostingDetailUseCase(IJobPostingRepository jobs, ICompanyRepository companies)
    { _jobs = jobs; _companies = companies; }

    public async Task<JobPostingDetailResponse> ExecuteAsync(Guid jobPostingId,
        CancellationToken cancellationToken = default)
    {
        var job = await _jobs.GetByIdAsync(jobPostingId, true, cancellationToken);
        // A posting that is not published is not public: it must not be readable by guessing its id.
        if (job is null || job.Status != JobPostingStatus.Published || (job.ApplicationDeadline is not null && job.ApplicationDeadline < DateOnly.FromDateTime(DateTime.UtcNow)))
            throw new KeyNotFoundException("Job posting was not found.");
        var company = await _companies.GetByIdAsync(job.CompanyId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Company was not found.");
        if (company.Status != CompanyStatus.Active) throw new KeyNotFoundException("Job posting was not found.");
        return new JobPostingDetailResponse(job.ToResponse(), company.ToResponse());
    }
}

public class GetCompanyProfileUseCase
{
    private readonly ICompanyRepository _companies;
    public GetCompanyProfileUseCase(ICompanyRepository companies) => _companies = companies;

    public async Task<CompanyResponse> ExecuteAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await _companies.GetByIdAsync(companyId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Company was not found.");
        return company.ToResponse();
    }
}

public class GetMyCompaniesUseCase
{
    private readonly ICompanyRepository _companies;
    public GetMyCompaniesUseCase(ICompanyRepository companies) => _companies = companies;

    public async Task<IReadOnlyList<CompanyMembershipResponse>> ExecuteAsync(Guid currentUserId,
        CancellationToken cancellationToken = default) =>
        (await _companies.GetMembershipsByUserAsync(currentUserId, cancellationToken))
            .Select(x => new CompanyMembershipResponse(x.Company.ToResponse(), x.Member.Id, x.Member.Role,
                x.Member.IsActive)).ToList();
}

public class GetCompanyMembersUseCase
{
    private readonly ICompanyRepository _companies;
    public GetCompanyMembersUseCase(ICompanyRepository companies) => _companies = companies;

    public async Task<IReadOnlyList<CompanyMemberDetailResponse>> ExecuteAsync(Guid companyId, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var member = await _companies.GetMemberAsync(companyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive) throw new UnauthorizedAccessException();
        return (await _companies.GetMembersWithUsersAsync(companyId, cancellationToken))
            .Select(x => new CompanyMemberDetailResponse(x.Member.Id, x.Member.CompanyId, x.Member.UserId,
                x.User.Email, x.User.FullName, x.User.PhoneNumber, x.Member.Role, x.Member.IsActive)).ToList();
    }
}

public class GetApplicationDetailUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly IJobPostingRepository _jobs;
    private readonly ICompanyRepository _companies; private readonly ICandidateRepository _candidates;
    private readonly IUserRepository _users;
    public GetApplicationDetailUseCase(IJobApplicationRepository applications, IJobPostingRepository jobs,
        ICompanyRepository companies, ICandidateRepository candidates, IUserRepository users)
    { _applications = applications; _jobs = jobs; _companies = companies; _candidates = candidates; _users = users; }

    public async Task<JobApplicationDetailResponse> ExecuteAsync(Guid applicationId, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var application = await _applications.GetDetailByIdAsync(applicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application was not found.");
        var job = await _jobs.GetByIdAsync(application.JobPostingId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Job posting was not found.");
        var profile = await _candidates.GetProfileByIdAsync(application.CandidateProfileId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");

        var isOwningCandidate = profile.UserId == currentUserId;
        CompanyMember? member = null;
        if (!isOwningCandidate)
        {
            member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
            var permitted = member is not null && member.IsActive &&
                (member.Role is CompanyMemberRole.Owner or CompanyMemberRole.HR ||
                 application.AssignedRecruiterMemberId == member.Id);
            if (!permitted) throw new UnauthorizedAccessException();
        }

        var company = await _companies.GetByIdAsync(job.CompanyId, cancellationToken: cancellationToken);
        var resume = profile.Resumes.FirstOrDefault(x => x.Id == application.ResumeId);
        // The candidate profile is disclosed to the hiring side only; the candidate reads it from their own profile.
        CandidateSummaryResponse? candidate = null;
        if (member is not null)
        {
            var user = await _users.GetByIdAsync(profile.UserId, cancellationToken);
            candidate = profile.ToSummary(user, resume);
        }

        return new JobApplicationDetailResponse(application.ToResponse(), job.ToSummary(company), candidate,
            application.Interviews.OrderByDescending(x => x.StartsAtUtc).Select(x => x.ToResponse()).ToList(),
            application.Offers.OrderByDescending(x => x.CreatedAtUtc).Select(x => x.ToResponse()).ToList(),
            application.StatusHistory.OrderByDescending(x => x.CreatedAtUtc).Select(x => x.ToResponse()).ToList());
    }
}

public class GetAdminCompaniesUseCase
{
    private readonly IUserRepository _users; private readonly ICompanyRepository _companies;
    public GetAdminCompaniesUseCase(IUserRepository users, ICompanyRepository companies)
    { _users = users; _companies = companies; }

    public async Task<IReadOnlyList<CompanyResponse>> ExecuteAsync(CompanyStatus? status, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken)
            ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin) throw new UnauthorizedAccessException();
        return (await _companies.GetAllAsync(status, cancellationToken)).Select(x => x.ToResponse()).ToList();
    }
}

public class GetAdminJobPostingsUseCase
{
    private readonly IUserRepository _users; private readonly IJobPostingRepository _jobs;
    private readonly ICompanyRepository _companies;
    public GetAdminJobPostingsUseCase(IUserRepository users, IJobPostingRepository jobs, ICompanyRepository companies)
    { _users = users; _jobs = jobs; _companies = companies; }

    public async Task<IReadOnlyList<JobPostingSummaryResponse>> ExecuteAsync(JobPostingStatus? status,
        Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken)
            ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin) throw new UnauthorizedAccessException();
        var jobs = await _jobs.GetAllAsync(status, cancellationToken);
        var companies = (await _companies.GetByIdsAsync(jobs.Select(x => x.CompanyId).Distinct().ToList(),
            cancellationToken)).ToDictionary(x => x.Id);
        return jobs.Select(x => x.ToSummary(companies.GetValueOrDefault(x.CompanyId))).ToList();
    }
}
