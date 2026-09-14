using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
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

    public Task AddAsync(Company company, CancellationToken cancellationToken = default) =>
        _context.Companies.AddAsync(company, cancellationToken).AsTask();

    public Task AddMemberAsync(CompanyMember member, CancellationToken cancellationToken = default) =>
        _context.CompanyMembers.AddAsync(member, cancellationToken).AsTask();
    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Companies.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
}
