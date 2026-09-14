using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.Candidates;

public class AddResumeUseCase
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;
    public AddResumeUseCase(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    { _candidates = candidates; _unitOfWork = unitOfWork; }

    public async Task<ResumeResponse> ExecuteAsync(AddResumeRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        if (request.IsDefault)
            foreach (var existingResume in profile.Resumes) existingResume.IsDefault = false;

        var resume = new Resume { CandidateProfileId = profile.Id, Name = request.Name.Trim(), FileUrl = request.FileUrl.Trim(), IsDefault = request.IsDefault };
        await _candidates.AddResumeAsync(resume, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ResumeResponse(resume.Id, resume.Name, resume.FileUrl, resume.IsDefault);
    }
}
