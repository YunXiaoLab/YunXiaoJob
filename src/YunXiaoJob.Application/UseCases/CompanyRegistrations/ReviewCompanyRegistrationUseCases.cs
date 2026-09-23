using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Domain.Enums;

namespace YunXiaoJob.Application.UseCases.CompanyRegistrations;

public class GetCompanyRegistrationsUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    public GetCompanyRegistrationsUseCase(IUserRepository users, ICompanyRepository companies)
    { _users = users; _companies = companies; }

    public async Task<IReadOnlyList<CompanyRegistrationResponse>> ExecuteAsync(CompanyRegistrationStatus? status,
        Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken) ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin)
            throw new UnauthorizedAccessException("Only an admin can review company registrations.");
        return (await _companies.GetRegistrationsAsync(status, cancellationToken)).Select(x => x.ToResponse()).ToList();
    }
}

public class ApproveCompanyRegistrationUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IPasswordService _passwords;
    private readonly IEmailService _email;
    private readonly IUnitOfWork _unitOfWork;
    public ApproveCompanyRegistrationUseCase(IUserRepository users, ICompanyRepository companies,
        IPasswordService passwords, IEmailService email, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _passwords = passwords; _email = email; _unitOfWork = unitOfWork; }

    public async Task<CompanyRegistrationResponse> ExecuteAsync(Guid registrationId,
        ReviewCompanyRegistrationRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken) ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin)
            throw new UnauthorizedAccessException("Only an admin can approve a company registration.");
        var registration = await _companies.GetRegistrationByIdAsync(registrationId, cancellationToken)
            ?? throw new KeyNotFoundException("Company registration was not found.");
        if (registration.Status != CompanyRegistrationStatus.Pending)
            throw new InvalidOperationException("Only a pending registration can be reviewed.");
        if (await _users.ExistsByEmailAsync(registration.ContactEmail, cancellationToken))
            throw new InvalidOperationException("Email is already in use.");

        var company = new Company
        {
            Name = registration.CompanyName, Description = registration.Description, Website = registration.Website,
            Address = registration.Address, Industry = registration.Industry,
            EmployeeCount = registration.EmployeeCount, Status = CompanyStatus.Active
        };
        var temporaryPassword = _passwords.GenerateTemporaryPassword();
        var owner = new User
        {
            Email = registration.ContactEmail, FullName = registration.ContactFullName,
            PhoneNumber = registration.ContactPhoneNumber, PasswordHash = _passwords.HashPassword(temporaryPassword)
        };
        var member = new CompanyMember { CompanyId = company.Id, UserId = owner.Id, Role = CompanyMemberRole.Owner };

        registration.Status = CompanyRegistrationStatus.Approved;
        registration.ReviewNote = request?.Note?.Trim();
        registration.ReviewedByUserId = admin.Id;
        registration.ReviewedAtUtc = DateTime.UtcNow;
        registration.CreatedCompanyId = company.Id;
        registration.CreatedOwnerUserId = owner.Id;
        registration.UpdatedAtUtc = registration.ReviewedAtUtc;

        await _users.AddAsync(owner, cancellationToken);
        await _companies.AddAsync(company, cancellationToken);
        await _companies.AddMemberAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _email.SendCompanyAccountCredentialsAsync(owner.Email, owner.FullName, company.Name,
            CompanyMemberRole.Owner, temporaryPassword, cancellationToken);
        return registration.ToResponse();
    }
}

public class RejectCompanyRegistrationUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IEmailService _email;
    private readonly IUnitOfWork _unitOfWork;
    public RejectCompanyRegistrationUseCase(IUserRepository users, ICompanyRepository companies,
        IEmailService email, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _email = email; _unitOfWork = unitOfWork; }

    public async Task<CompanyRegistrationResponse> ExecuteAsync(Guid registrationId,
        ReviewCompanyRegistrationRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var admin = await _users.GetByIdAsync(currentUserId, cancellationToken) ?? throw new UnauthorizedAccessException();
        if (admin.PlatformRole != PlatformRole.Admin)
            throw new UnauthorizedAccessException("Only an admin can reject a company registration.");
        var registration = await _companies.GetRegistrationByIdAsync(registrationId, cancellationToken)
            ?? throw new KeyNotFoundException("Company registration was not found.");
        if (registration.Status != CompanyRegistrationStatus.Pending)
            throw new InvalidOperationException("Only a pending registration can be reviewed.");

        registration.Status = CompanyRegistrationStatus.Rejected;
        registration.ReviewNote = request?.Note?.Trim();
        registration.ReviewedByUserId = admin.Id;
        registration.ReviewedAtUtc = DateTime.UtcNow;
        registration.UpdatedAtUtc = registration.ReviewedAtUtc;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _email.SendCompanyRegistrationRejectedAsync(registration.ContactEmail, registration.ContactFullName,
            registration.CompanyName, registration.ReviewNote, cancellationToken);
        return registration.ToResponse();
    }
}
