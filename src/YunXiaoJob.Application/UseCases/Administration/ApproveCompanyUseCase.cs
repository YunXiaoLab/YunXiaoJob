using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Administration;

public class ApproveCompanyUseCase
{
    private readonly IUserRepository _users; private readonly ICompanyRepository _companies; private readonly IUnitOfWork _unitOfWork;
    public ApproveCompanyUseCase(IUserRepository users, ICompanyRepository companies, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _unitOfWork = unitOfWork; }
    public async Task<CompanyResponse> ExecuteAsync(Guid companyId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken) ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin) throw new UnauthorizedAccessException("Only an admin can approve a company.");
        var company = await _companies.GetByIdAsync(companyId, cancellationToken: cancellationToken) ?? throw new KeyNotFoundException("Company was not found.");
        if (company.Status != CompanyStatus.PendingVerification) throw new InvalidOperationException("Only a pending company can be approved.");
        company.Status = CompanyStatus.Active; company.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CompanyResponse(company.Id, company.Name, company.LogoUrl, company.Description, company.Website, company.Address, company.Industry, company.EmployeeCount, company.Status);
    }
}
