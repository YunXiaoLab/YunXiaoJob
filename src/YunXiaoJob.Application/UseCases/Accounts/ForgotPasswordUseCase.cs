using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;
namespace YunXiaoJob.Application.UseCases.Accounts;
public class ForgotPasswordUseCase
{
    private readonly IUserRepository _users; private readonly ICryptographyService _crypto; private readonly IEmailService _email; private readonly IUnitOfWork _unitOfWork;
    public ForgotPasswordUseCase(IUserRepository users, ICryptographyService crypto, IEmailService email, IUnitOfWork unitOfWork) { _users = users; _crypto = crypto; _email = email; _unitOfWork = unitOfWork; }
    public async Task ExecuteAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken); if (user is null || !user.IsActive) return;
        var token = _crypto.GenerateSecureToken(); await _users.AddPasswordResetTokenAsync(new PasswordResetToken { UserId = user.Id, TokenHash = _crypto.ComputeHash(token), ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30) }, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); await _email.SendPasswordResetAsync(user.Email, token, cancellationToken);
    }
}
