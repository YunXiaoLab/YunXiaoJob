using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;
using YunXiaoJob.Infrastructure.Data;

namespace YunXiaoJob.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;
    public CompanyRepository(ApplicationDbContext context) => _context = context;

    public Task<Company?> GetByIdAsync(Guid companyId, bool includeMembers = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Company> query = _context.Companies;
        if (includeMembers) query = query.Include(x => x.Members);
        return query.FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken);
    }

    public Task<CompanyMember?> GetMemberAsync(Guid companyId, Guid userId,
        CancellationToken cancellationToken = default) =>
        _context.CompanyMembers.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.UserId == userId,
            cancellationToken);

    public Task<CompanyMember?> GetMemberByIdAsync(Guid memberId,
        CancellationToken cancellationToken = default) =>
        _context.CompanyMembers.FirstOrDefaultAsync(x => x.Id == memberId, cancellationToken);

    public Task<bool> HasActiveMemberAsync(Guid companyId, Guid userId,
        CancellationToken cancellationToken = default) =>
        _context.CompanyMembers.AnyAsync(x => x.CompanyId == companyId && x.UserId == userId && x.IsActive,
            cancellationToken);

    public async Task<IReadOnlyList<Company>> GetAllAsync(CompanyStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Company> query = _context.Companies;
        if (status is not null) query = query.Where(x => x.Status == status);
        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Company>> GetByIdsAsync(IReadOnlyCollection<Guid> companyIds,
        CancellationToken cancellationToken = default) =>
        companyIds.Count == 0
            ? []
            : await _context.Companies.Where(x => companyIds.Contains(x.Id)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<(Company Company, CompanyMember Member)>> GetMembershipsByUserAsync(Guid userId,
        CancellationToken cancellationToken = default) =>
        (await _context.CompanyMembers
            .Where(member => member.UserId == userId)
            .Join(_context.Companies, member => member.CompanyId, company => company.Id,
                (member, company) => new { Member = member, Company = company })
            .OrderBy(x => x.Company.Name)
            .ToListAsync(cancellationToken))
            .Select(x => (x.Company, x.Member)).ToList();

    public async Task<IReadOnlyList<(CompanyMember Member, User User)>> GetMembersWithUsersAsync(Guid companyId,
        CancellationToken cancellationToken = default) =>
        (await _context.CompanyMembers
            .Where(member => member.CompanyId == companyId)
            .Join(_context.Users, member => member.UserId, user => user.Id,
                (member, user) => new { Member = member, User = user })
            .OrderBy(x => x.Member.Role).ThenBy(x => x.User.FullName)
            .ToListAsync(cancellationToken))
            .Select(x => (x.Member, x.User)).ToList();

    public Task AddAsync(Company company, CancellationToken cancellationToken = default) =>
        _context.Companies.AddAsync(company, cancellationToken).AsTask();

    public Task AddMemberAsync(CompanyMember member, CancellationToken cancellationToken = default) =>
        _context.CompanyMembers.AddAsync(member, cancellationToken).AsTask();

    public Task<CompanyRegistration?> GetRegistrationByIdAsync(Guid registrationId,
        CancellationToken cancellationToken = default) =>
        _context.CompanyRegistrations.FirstOrDefaultAsync(x => x.Id == registrationId, cancellationToken);

    public async Task<IReadOnlyList<CompanyRegistration>> GetRegistrationsAsync(CompanyRegistrationStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<CompanyRegistration> query = _context.CompanyRegistrations;
        if (status is not null) query = query.Where(x => x.Status == status);
        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public Task<bool> HasPendingRegistrationAsync(string contactEmail,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = contactEmail.Trim().ToLowerInvariant();
        return _context.CompanyRegistrations.AnyAsync(
            x => x.ContactEmail == normalizedEmail && x.Status == CompanyRegistrationStatus.Pending,
            cancellationToken);
    }

    public Task AddRegistrationAsync(CompanyRegistration registration,
        CancellationToken cancellationToken = default) =>
        _context.CompanyRegistrations.AddAsync(registration, cancellationToken).AsTask();
}
