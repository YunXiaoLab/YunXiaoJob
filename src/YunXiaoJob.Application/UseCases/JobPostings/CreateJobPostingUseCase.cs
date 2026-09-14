using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.JobPostings;

public class CreateJobPostingUseCase
{
    private readonly ICompanyRepository _companies;
    private readonly IJobPostingRepository _jobs;
    private readonly IUnitOfWork _unitOfWork;
    public CreateJobPostingUseCase(ICompanyRepository companies, IJobPostingRepository jobs, IUnitOfWork unitOfWork)
    { _companies = companies; _jobs = jobs; _unitOfWork = unitOfWork; }

    public async Task<JobPostingResponse> ExecuteAsync(CreateJobPostingRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var member = await _companies.GetMemberAsync(request.CompanyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive || member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR))
            throw new UnauthorizedAccessException("Only an owner or HR member can create a job posting.");
        var company = await _companies.GetByIdAsync(request.CompanyId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Company was not found.");
        if (company.Status != CompanyStatus.Active) throw new InvalidOperationException("Company is not active.");
        if (request.MinSalary.HasValue && request.MaxSalary.HasValue && request.MinSalary > request.MaxSalary)
            throw new InvalidOperationException("Minimum salary cannot exceed maximum salary.");

        var job = new JobPosting
        {
            CompanyId = company.Id, CreatedByMemberId = member.Id, Title = request.Title.Trim(),
            Description = request.Description.Trim(), Requirements = request.Requirements.Trim(),
            Benefits = request.Benefits?.Trim(), Location = request.Location.Trim(),
            EmploymentType = request.EmploymentType, WorkplaceType = request.WorkplaceType,
            MinSalary = request.MinSalary, MaxSalary = request.MaxSalary, Currency = request.Currency.Trim().ToUpperInvariant(),
            ApplicationDeadline = request.ApplicationDeadline, Status = JobPostingStatus.Draft,
            Skills = request.Skills.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(x => new JobPostingSkill { Name = x.Trim() }).ToList()
        };
        foreach (var skill in job.Skills) skill.JobPostingId = job.Id;
        await _jobs.AddAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobPostingResponse(job.Id, job.CompanyId, job.CreatedByMemberId, job.AssignedRecruiterMemberId,
            job.Title, job.Description, job.Requirements, job.Benefits, job.Location, job.EmploymentType,
            job.WorkplaceType, job.MinSalary, job.MaxSalary, job.Currency, job.ApplicationDeadline, job.Status,
            job.PublishedAtUtc, job.Skills.Select(x => x.Name).ToList());
    }
}
