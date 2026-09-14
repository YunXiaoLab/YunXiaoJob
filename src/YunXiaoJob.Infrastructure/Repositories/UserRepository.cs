using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Infrastructure.Data;

namespace YunXiaoJob.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _context.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _context.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        _context.Users.AddAsync(user, cancellationToken).AsTask();
    public Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default) => _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    public Task<PasswordResetToken?> GetPasswordResetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default) => _context.PasswordResetTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    public Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default) => _context.RefreshTokens.AddAsync(token, cancellationToken).AsTask();
    public Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken = default) => _context.PasswordResetTokens.AddAsync(token, cancellationToken).AsTask();
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Users.OrderBy(x => x.Email).ToListAsync(cancellationToken);
    public Task<RegistrationChallenge?> GetRegistrationChallengeByIdAsync(Guid challengeId, CancellationToken cancellationToken = default) => _context.RegistrationChallenges.FirstOrDefaultAsync(x => x.Id == challengeId, cancellationToken);
    public Task<RegistrationChallenge?> GetRegistrationChallengeByEmailAsync(string email, CancellationToken cancellationToken = default) => _context.RegistrationChallenges.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    public Task AddRegistrationChallengeAsync(RegistrationChallenge challenge, CancellationToken cancellationToken = default) => _context.RegistrationChallenges.AddAsync(challenge, cancellationToken).AsTask();
    public void RemoveRegistrationChallenge(RegistrationChallenge challenge) => _context.RegistrationChallenges.Remove(challenge);
}
