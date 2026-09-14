using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.JobPostings;

public class UpdateJobPostingUseCase
{
    private readonly ICompanyRepository _companies;
    private readonly IJobPostingRepository _jobs;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateJobPostingUseCase(ICompanyRepository companies, IJobPostingRepository jobs, IUnitOfWork unitOfWork)
    { _companies = companies; _jobs = jobs; _unitOfWork = unitOfWork; }

    public async Task<JobPostingResponse> ExecuteAsync(Guid jobPostingId, UpdateJobPostingRequest request,
        Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var job = await _jobs.GetByIdAsync(jobPostingId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        var canManage = member is not null && member.IsActive &&
            (member.Role is CompanyMemberRole.Owner or CompanyMemberRole.HR || job.AssignedRecruiterMemberId == member.Id);
        if (!canManage) throw new UnauthorizedAccessException("You cannot update this job posting.");
        if (job.Status is JobPostingStatus.Closed or JobPostingStatus.Expired)
            throw new InvalidOperationException("Closed or expired job postings cannot be updated.");
        if (request.MinSalary.HasValue && request.MaxSalary.HasValue && request.MinSalary > request.MaxSalary)
            throw new InvalidOperationException("Minimum salary cannot exceed maximum salary.");

        job.Title = request.Title.Trim(); job.Description = request.Description.Trim(); job.Requirements = request.Requirements.Trim();
        job.Benefits = request.Benefits?.Trim(); job.Location = request.Location.Trim(); job.EmploymentType = request.EmploymentType;
        job.WorkplaceType = request.WorkplaceType; job.MinSalary = request.MinSalary; job.MaxSalary = request.MaxSalary;
        job.Currency = request.Currency.Trim().ToUpperInvariant(); job.ApplicationDeadline = request.ApplicationDeadline;
        job.UpdatedAtUtc = DateTime.UtcNow;
        foreach (var skill in job.Skills.ToList()) _jobs.RemoveSkill(skill);
        foreach (var skillName in request.Skills.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
            await _jobs.AddSkillAsync(new JobPostingSkill { JobPostingId = job.Id, Name = skillName.Trim() }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobPostingResponse(job.Id, job.CompanyId, job.CreatedByMemberId, job.AssignedRecruiterMemberId,
            job.Title, job.Description, job.Requirements, job.Benefits, job.Location, job.EmploymentType,
            job.WorkplaceType, job.MinSalary, job.MaxSalary, job.Currency, job.ApplicationDeadline, job.Status,
            job.PublishedAtUtc, request.Skills.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList());
    }
}
