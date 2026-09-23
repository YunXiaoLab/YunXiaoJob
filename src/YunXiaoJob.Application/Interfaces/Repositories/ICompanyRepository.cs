using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

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
    Task<IReadOnlyList<Company>> GetAllAsync(CompanyStatus? status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Company>> GetByIdsAsync(IReadOnlyCollection<Guid> companyIds,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(Company Company, CompanyMember Member)>> GetMembershipsByUserAsync(Guid userId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(CompanyMember Member, User User)>> GetMembersWithUsersAsync(Guid companyId,
        CancellationToken cancellationToken = default);
    Task AddAsync(Company company, CancellationToken cancellationToken = default);
    Task AddMemberAsync(CompanyMember member, CancellationToken cancellationToken = default);
    Task<CompanyRegistration?> GetRegistrationByIdAsync(Guid registrationId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompanyRegistration>> GetRegistrationsAsync(CompanyRegistrationStatus? status,
        CancellationToken cancellationToken = default);
    Task<bool> HasPendingRegistrationAsync(string contactEmail, CancellationToken cancellationToken = default);
    Task AddRegistrationAsync(CompanyRegistration registration, CancellationToken cancellationToken = default);
}
