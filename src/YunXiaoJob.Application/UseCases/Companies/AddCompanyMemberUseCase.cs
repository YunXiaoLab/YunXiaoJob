using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Companies;

public class AddCompanyMemberUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _unitOfWork;
    public AddCompanyMemberUseCase(IUserRepository users, ICompanyRepository companies, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _unitOfWork = unitOfWork; }

    public async Task<CompanyMemberResponse> ExecuteAsync(Guid companyId, AddCompanyMemberRequest request,
        Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var owner = await _companies.GetMemberAsync(companyId, currentUserId, cancellationToken);
        if (owner is null || !owner.IsActive || owner.Role != CompanyMemberRole.Owner)
            throw new UnauthorizedAccessException("Only the company owner can add members.");
        _ = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("User to add was not found.");
        if (await _companies.HasActiveMemberAsync(companyId, request.UserId, cancellationToken))
            throw new InvalidOperationException("User is already an active company member.");
        var member = new CompanyMember { CompanyId = companyId, UserId = request.UserId, Role = request.Role };
        await _companies.AddMemberAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CompanyMemberResponse(member.Id, member.CompanyId, member.UserId, member.Role, member.IsActive);
    }
}
