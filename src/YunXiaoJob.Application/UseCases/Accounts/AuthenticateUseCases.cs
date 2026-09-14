using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.Accounts;

public class LoginUseCase
{
    private readonly IUserRepository _users; private readonly IPasswordService _passwords; private readonly IAuthTokenService _tokens; private readonly ICryptographyService _crypto; private readonly IUnitOfWork _unitOfWork;
    public LoginUseCase(IUserRepository users, IPasswordService passwords, IAuthTokenService tokens, ICryptographyService crypto, IUnitOfWork unitOfWork) { _users = users; _passwords = passwords; _tokens = tokens; _crypto = crypto; _unitOfWork = unitOfWork; }
    public async Task<AuthenticationResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.IsActive || !_passwords.VerifyPassword(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid credentials.");
        var refresh = _crypto.GenerateSecureToken(); var expires = DateTime.UtcNow.AddDays(30);
        await _users.AddRefreshTokenAsync(new RefreshToken { UserId = user.Id, TokenHash = _crypto.ComputeHash(refresh), ExpiresAtUtc = expires }, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AuthenticationResponse(_tokens.CreateAccessToken(user.Id, user.Email, user.PlatformRole), refresh, expires, new UserResponse(user.Id, user.Email, user.FullName, user.PhoneNumber, user.PlatformRole, user.IsActive));
    }
}

public class RefreshAccessTokenUseCase
{
    private readonly IUserRepository _users; private readonly IAuthTokenService _tokens; private readonly ICryptographyService _crypto; private readonly IUnitOfWork _unitOfWork;
    public RefreshAccessTokenUseCase(IUserRepository users, IAuthTokenService tokens, ICryptographyService crypto, IUnitOfWork unitOfWork) { _users = users; _tokens = tokens; _crypto = crypto; _unitOfWork = unitOfWork; }
    public async Task<AuthenticationResponse> ExecuteAsync(RefreshAccessTokenRequest request, CancellationToken cancellationToken = default)
    {
        var token = await _users.GetRefreshTokenByHashAsync(_crypto.ComputeHash(request.RefreshToken), cancellationToken);
        if (token is null || token.RevokedAtUtc is not null || token.ExpiresAtUtc <= DateTime.UtcNow) throw new UnauthorizedAccessException("Refresh token is invalid.");
        var user = await _users.GetByIdAsync(token.UserId, cancellationToken) ?? throw new UnauthorizedAccessException(); token.RevokedAtUtc = DateTime.UtcNow;
        var refresh = _crypto.GenerateSecureToken(); var expires = DateTime.UtcNow.AddDays(30); await _users.AddRefreshTokenAsync(new RefreshToken { UserId = user.Id, TokenHash = _crypto.ComputeHash(refresh), ExpiresAtUtc = expires }, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AuthenticationResponse(_tokens.CreateAccessToken(user.Id, user.Email, user.PlatformRole), refresh, expires, new UserResponse(user.Id, user.Email, user.FullName, user.PhoneNumber, user.PlatformRole, user.IsActive));
    }
}

public class ResetPasswordUseCase
{
    private readonly IUserRepository _users; private readonly IPasswordService _passwords; private readonly ICryptographyService _crypto; private readonly IUnitOfWork _unitOfWork;
    public ResetPasswordUseCase(IUserRepository users, IPasswordService passwords, ICryptographyService crypto, IUnitOfWork unitOfWork) { _users = users; _passwords = passwords; _crypto = crypto; _unitOfWork = unitOfWork; }
    public async Task ExecuteAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var reset = await _users.GetPasswordResetTokenByHashAsync(_crypto.ComputeHash(request.Token), cancellationToken);
        if (reset is null || reset.UsedAtUtc is not null || reset.ExpiresAtUtc <= DateTime.UtcNow) throw new InvalidOperationException("Password reset token is invalid.");
        var user = await _users.GetByIdAsync(reset.UserId, cancellationToken) ?? throw new KeyNotFoundException("User was not found.");
        user.PasswordHash = _passwords.HashPassword(request.NewPassword); user.UpdatedAtUtc = DateTime.UtcNow; reset.UsedAtUtc = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
