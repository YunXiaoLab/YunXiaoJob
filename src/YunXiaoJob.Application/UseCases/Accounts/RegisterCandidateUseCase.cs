using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.Accounts;

public class RegisterCandidateUseCase
{
    private readonly IUserRepository _users;
    private readonly ICandidateRepository _candidates;
    private readonly IPasswordService _passwords;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCandidateUseCase(IUserRepository users, ICandidateRepository candidates,
        IPasswordService passwords, IUnitOfWork unitOfWork)
    {
        _users = users;
        _candidates = candidates;
        _passwords = passwords;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> ExecuteAsync(RegisterCandidateRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _users.ExistsByEmailAsync(email, cancellationToken))
            throw new InvalidOperationException("Email is already in use.");

        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = _passwords.HashPassword(request.Password)
        };
        var profile = new CandidateProfile { UserId = user.Id };

        await _users.AddAsync(user, cancellationToken);
        await _candidates.AddProfileAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.Id, user.Email, user.FullName, user.PhoneNumber, user.PlatformRole, user.IsActive);
    }
}
