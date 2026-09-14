using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class CreateJobOfferUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly IJobPostingRepository _jobs; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unitOfWork;
    public CreateJobOfferUseCase(IJobApplicationRepository applications, IJobPostingRepository jobs, ICompanyRepository companies, IUnitOfWork unitOfWork) { _applications = applications; _jobs = jobs; _companies = companies; _unitOfWork = unitOfWork; }
    public async Task<JobOfferResponse> ExecuteAsync(Guid applicationId, CreateJobOfferRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var application = await _applications.GetByIdAsync(applicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        var job = await _jobs.GetByIdAsync(application.JobPostingId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Job posting was not found.");
        var member = await _companies.GetMemberAsync(job.CompanyId, currentUserId, cancellationToken);
        if (member is null || !member.IsActive || member.Role is not (CompanyMemberRole.Owner or CompanyMemberRole.HR)) throw new UnauthorizedAccessException();
        var offer = new JobOffer { JobApplicationId = application.Id, CreatedByMemberId = member.Id, Salary = request.Salary, Currency = request.Currency.Trim().ToUpperInvariant(), StartDate = request.StartDate, ExpiresOn = request.ExpiresOn, Note = request.Note?.Trim(), Status = OfferStatus.Sent };
        var previous = application.Status; application.Status = ApplicationStatus.Offered; application.UpdatedAtUtc = DateTime.UtcNow;
        await _applications.AddOfferAsync(offer, cancellationToken); await _applications.AddStatusHistoryAsync(new ApplicationStatusHistory { JobApplicationId = application.Id, FromStatus = previous, ToStatus = application.Status, ChangedByMemberId = member.Id, Note = "Offer sent." }, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobOfferResponse(offer.Id, offer.JobApplicationId, offer.Salary, offer.Currency, offer.StartDate, offer.ExpiresOn, offer.Note, offer.Status);
    }
}
