using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid companyId, bool includeMembers = false,
        CancellationToken cancellationToken = default);
    Task<CompanyMember?> GetMemberAsync(Guid companyId, Guid userId,
        CancellationToken cancellationToken = default);
    Task<CompanyMember?> GetMemberByIdAsync(Guid memberId,
        CancellationToken cancellationToken = default);
    Task<bool> HasActiveMemberAsync(Guid companyId, Guid userId,
        CancellationToken cancellationToken = default);
    Task AddAsync(Company company, CancellationToken cancellationToken = default);
    Task AddMemberAsync(CompanyMember member, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default);
}
