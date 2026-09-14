using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Companies;

public class CreateCompanyUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCompanyUseCase(IUserRepository users, ICompanyRepository companies, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _unitOfWork = unitOfWork; }

    public async Task<CompanyResponse> ExecuteAsync(CreateCompanyRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        _ = await _users.GetByIdAsync(currentUserId, cancellationToken)
            ?? throw new KeyNotFoundException("Current user was not found.");
        var company = new Company
        {
            Name = request.Name.Trim(), Description = request.Description?.Trim(), Website = request.Website?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(), Address = request.Address?.Trim(), Industry = request.Industry?.Trim(),
            EmployeeCount = request.EmployeeCount, Status = CompanyStatus.PendingVerification
        };
        var owner = new CompanyMember { CompanyId = company.Id, UserId = currentUserId, Role = CompanyMemberRole.Owner };
        await _companies.AddAsync(company, cancellationToken);
        await _companies.AddMemberAsync(owner, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CompanyResponse(company.Id, company.Name, company.LogoUrl, company.Description, company.Website,
            company.Address, company.Industry, company.EmployeeCount, company.Status);
    }
}
