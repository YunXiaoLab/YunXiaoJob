using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.UseCases.Queries;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.JobPostings;

public sealed class PauseJobPostingUseCase
{
    private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unit;
    public PauseJobPostingUseCase(IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unit) { _jobs = jobs; _companies = companies; _unit = unit; }
    public async Task<JobPostingResponse> ExecuteAsync(Guid id, Guid userId, CancellationToken ct = default)
    { var job = await _jobs.GetByIdAsync(id, true, ct) ?? throw new KeyNotFoundException("Job posting was not found."); var member = await _companies.GetMemberAsync(job.CompanyId, userId, ct); if (member is null || !member.IsActive || member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR)) throw new UnauthorizedAccessException(); if (job.Status != JobPostingStatus.Published) throw new InvalidOperationException("Only a published posting can be paused."); job.Status = JobPostingStatus.Paused; job.UpdatedAtUtc = DateTime.UtcNow; await _unit.SaveChangesAsync(ct); return job.ToResponse(); }
}

public sealed class CloseJobPostingUseCase
{
    private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unit;
    public CloseJobPostingUseCase(IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unit) { _jobs = jobs; _companies = companies; _unit = unit; }
    public async Task<JobPostingResponse> ExecuteAsync(Guid id, Guid userId, CancellationToken ct = default)
    { var job = await _jobs.GetByIdAsync(id, true, ct) ?? throw new KeyNotFoundException("Job posting was not found."); var member = await _companies.GetMemberAsync(job.CompanyId, userId, ct); if (member is null || !member.IsActive || member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR)) throw new UnauthorizedAccessException(); if (job.Status is JobPostingStatus.Closed or JobPostingStatus.Expired) throw new InvalidOperationException("Job posting is already closed."); job.Status = JobPostingStatus.Closed; job.ClosedAtUtc = job.UpdatedAtUtc = DateTime.UtcNow; await _unit.SaveChangesAsync(ct); return job.ToResponse(); }
}

public sealed class ResumeJobPostingUseCase
{
    private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unit;
    public ResumeJobPostingUseCase(IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unit) { _jobs = jobs; _companies = companies; _unit = unit; }
    public async Task<JobPostingResponse> ExecuteAsync(Guid id, Guid userId, CancellationToken ct = default)
    { var job = await _jobs.GetByIdAsync(id, true, ct) ?? throw new KeyNotFoundException("Job posting was not found."); var member = await _companies.GetMemberAsync(job.CompanyId, userId, ct); if (member is null || !member.IsActive || member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR)) throw new UnauthorizedAccessException(); var company = await _companies.GetByIdAsync(job.CompanyId, cancellationToken: ct) ?? throw new KeyNotFoundException("Company was not found."); if (company.Status != CompanyStatus.Active || job.Status != JobPostingStatus.Paused) throw new InvalidOperationException("Only a paused job of an active company can be resumed."); if (job.ApplicationDeadline is not null && job.ApplicationDeadline < DateOnly.FromDateTime(DateTime.UtcNow)) { job.Status = JobPostingStatus.Expired; throw new InvalidOperationException("An expired job cannot be resumed."); } job.Status = JobPostingStatus.Published; job.UpdatedAtUtc = DateTime.UtcNow; await _unit.SaveChangesAsync(ct); return job.ToResponse(); }
}
