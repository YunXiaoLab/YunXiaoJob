using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByIdsAsync(IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<PasswordResetToken?> GetPasswordResetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokensByUserIdAsync(Guid userId, DateTime revokedAtUtc, CancellationToken cancellationToken = default);
    Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RegistrationChallenge?> GetRegistrationChallengeByIdAsync(Guid challengeId, CancellationToken cancellationToken = default);
    Task<RegistrationChallenge?> GetRegistrationChallengeByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddRegistrationChallengeAsync(RegistrationChallenge challenge, CancellationToken cancellationToken = default);
    void RemoveRegistrationChallenge(RegistrationChallenge challenge);
}
