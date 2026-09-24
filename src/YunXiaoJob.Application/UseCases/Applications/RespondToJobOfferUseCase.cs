using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Applications;

public class RespondToJobOfferUseCase
{
    private readonly IJobApplicationRepository _applications; private readonly ICandidateRepository _candidates; private readonly IUnitOfWork _unitOfWork;
    public RespondToJobOfferUseCase(IJobApplicationRepository applications, ICandidateRepository candidates, IUnitOfWork unitOfWork) { _applications = applications; _candidates = candidates; _unitOfWork = unitOfWork; }
    public async Task<JobOfferResponse> ExecuteAsync(Guid offerId, RespondToJobOfferRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Candidate profile was not found.");
        var offer = await _applications.GetOfferByIdAsync(offerId, cancellationToken) ?? throw new KeyNotFoundException("Offer was not found.");
        var application = await _applications.GetByIdAsync(offer.JobApplicationId, cancellationToken) ?? throw new KeyNotFoundException("Application was not found.");
        if (application.CandidateProfileId != profile.Id || offer.Status != OfferStatus.Sent) throw new UnauthorizedAccessException();
        if (offer.ExpiresOn is not null && offer.ExpiresOn < DateOnly.FromDateTime(DateTime.UtcNow)) { offer.Status = OfferStatus.Expired; offer.UpdatedAtUtc = DateTime.UtcNow; if (application.Status == ApplicationStatus.Offered) { application.Status = ApplicationStatus.Interviewing; application.UpdatedAtUtc = offer.UpdatedAtUtc; } await _unitOfWork.SaveChangesAsync(cancellationToken); throw new InvalidOperationException("Offer has expired."); }
        offer.Status = request.Accept ? OfferStatus.Accepted : OfferStatus.Rejected; offer.UpdatedAtUtc = DateTime.UtcNow;
        var previous = application.Status; application.Status = request.Accept ? ApplicationStatus.Hired : ApplicationStatus.Rejected; application.UpdatedAtUtc = DateTime.UtcNow;
        await _applications.AddStatusHistoryAsync(new ApplicationStatusHistory { JobApplicationId = application.Id, FromStatus = previous, ToStatus = application.Status, Note = request.Accept ? "Offer accepted by candidate." : "Offer rejected by candidate." }, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new JobOfferResponse(offer.Id, offer.JobApplicationId, offer.Salary, offer.Currency, offer.StartDate, offer.ExpiresOn, offer.Note, offer.Status);
    }
}
