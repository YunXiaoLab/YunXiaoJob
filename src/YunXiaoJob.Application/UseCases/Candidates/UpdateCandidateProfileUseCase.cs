using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;

namespace YunXiaoJob.Application.UseCases.Candidates;

public class UpdateCandidateProfileUseCase
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCandidateProfileUseCase(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    { _candidates = candidates; _unitOfWork = unitOfWork; }

    public async Task<CandidateProfileResponse> ExecuteAsync(UpdateCandidateProfileRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        if (request.ExpectedMinSalary is < 0 || request.ExpectedMaxSalary is < 0)
            throw new InvalidOperationException("Expected salary cannot be negative.");
        if (request.ExpectedMinSalary is not null && request.ExpectedMaxSalary is not null &&
            request.ExpectedMinSalary > request.ExpectedMaxSalary)
            throw new InvalidOperationException("Expected minimum salary cannot exceed the maximum.");
        if (request.DateOfBirth is not null && request.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new InvalidOperationException("Date of birth cannot be in the future.");
        profile.Headline = request.Headline?.Trim();
        profile.Summary = request.Summary?.Trim();
        profile.Location = request.Location?.Trim();
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.IsSearchable = request.IsSearchable;
        profile.AvatarUrl = request.AvatarUrl?.Trim();
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = request.Gender;
        profile.ExpectedMinSalary = request.ExpectedMinSalary;
        profile.ExpectedMaxSalary = request.ExpectedMaxSalary;
        profile.ExpectedSalaryCurrency = string.IsNullOrWhiteSpace(request.ExpectedSalaryCurrency)
            ? null : request.ExpectedSalaryCurrency.Trim().ToUpperInvariant();
        profile.LinkedInUrl = request.LinkedInUrl?.Trim();
        profile.GitHubUrl = request.GitHubUrl?.Trim();
        profile.PortfolioUrl = request.PortfolioUrl?.Trim();
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile.ToResponse();
    }
}
