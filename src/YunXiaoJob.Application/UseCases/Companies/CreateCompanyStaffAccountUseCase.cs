using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.Companies;

public class CreateCompanyStaffAccountUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IPasswordService _passwords;
    private readonly IEmailService _email;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCompanyStaffAccountUseCase(IUserRepository users, ICompanyRepository companies,
        IPasswordService passwords, IEmailService email, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _passwords = passwords; _email = email; _unitOfWork = unitOfWork; }

    public async Task<CompanyStaffAccountResponse> ExecuteAsync(Guid companyId,
        CreateCompanyStaffAccountRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var owner = await _companies.GetMemberAsync(companyId, currentUserId, cancellationToken);
        if (owner is null || !owner.IsActive || owner.Role != CompanyMemberRole.Owner)
            throw new UnauthorizedAccessException("Only the company owner can create staff accounts.");
        if (request.Role is not (CompanyMemberRole.HR or CompanyMemberRole.Recruiter))
            throw new InvalidOperationException("A staff account must be an HR or a Recruiter.");
        var company = await _companies.GetByIdAsync(companyId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Company was not found.");
        if (company.Status != CompanyStatus.Active)
            throw new InvalidOperationException("Only an active company can create staff accounts.");
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new InvalidOperationException("Full name is required.");
        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email)) throw new InvalidOperationException("Email is required.");
        if (await _users.ExistsByEmailAsync(email, cancellationToken))
            throw new InvalidOperationException("Email is already in use.");

        var temporaryPassword = _passwords.GenerateTemporaryPassword();
        var user = new User
        {
            Email = email, FullName = request.FullName.Trim(), PhoneNumber = request.PhoneNumber?.Trim(),
            PasswordHash = _passwords.HashPassword(temporaryPassword)
        };
        var member = new CompanyMember { CompanyId = companyId, UserId = user.Id, Role = request.Role };
        await _users.AddAsync(user, cancellationToken);
        await _companies.AddMemberAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _email.SendCompanyAccountCredentialsAsync(user.Email, user.FullName, company.Name, request.Role,
            temporaryPassword, cancellationToken);
        return new CompanyStaffAccountResponse(member.Id, member.CompanyId, user.Id, user.Email, user.FullName,
            member.Role, member.IsActive);
    }
}
