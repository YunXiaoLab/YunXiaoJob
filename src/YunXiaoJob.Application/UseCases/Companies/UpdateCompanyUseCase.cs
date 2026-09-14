using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Companies;

public class UpdateCompanyUseCase
{
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCompanyUseCase(ICompanyRepository companies, IUnitOfWork unitOfWork)
    { _companies = companies; _unitOfWork = unitOfWork; }

    public async Task<CompanyResponse> ExecuteAsync(Guid companyId, UpdateCompanyRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var membership = await _companies.GetMemberAsync(companyId, currentUserId, cancellationToken);
        if (membership is null || !membership.IsActive || membership.Role != CompanyMemberRole.Owner)
            throw new UnauthorizedAccessException("Only the company owner can update the company profile.");
        var company = await _companies.GetByIdAsync(companyId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Company was not found.");
        company.Name = request.Name.Trim(); company.Description = request.Description?.Trim();
        company.Website = request.Website?.Trim(); company.LogoUrl = request.LogoUrl?.Trim();
        company.Address = request.Address?.Trim(); company.Industry = request.Industry?.Trim();
        company.EmployeeCount = request.EmployeeCount; company.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CompanyResponse(company.Id, company.Name, company.LogoUrl, company.Description, company.Website,
            company.Address, company.Industry, company.EmployeeCount, company.Status);
    }
}
