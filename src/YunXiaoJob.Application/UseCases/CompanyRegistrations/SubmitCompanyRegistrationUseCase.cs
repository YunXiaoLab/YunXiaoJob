using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.CompanyRegistrations;

public class SubmitCompanyRegistrationUseCase
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _unitOfWork;
    public SubmitCompanyRegistrationUseCase(IUserRepository users, ICompanyRepository companies, IUnitOfWork unitOfWork)
    { _users = users; _companies = companies; _unitOfWork = unitOfWork; }

    public async Task<CompanyRegistrationResponse> ExecuteAsync(SubmitCompanyRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName)) throw new InvalidOperationException("Company name is required.");
        if (string.IsNullOrWhiteSpace(request.ContactFullName)) throw new InvalidOperationException("Contact full name is required.");
        var email = request.ContactEmail?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email)) throw new InvalidOperationException("Contact email is required.");
        if (await _users.ExistsByEmailAsync(email, cancellationToken))
            throw new InvalidOperationException("Email is already in use.");
        if (await _companies.HasPendingRegistrationAsync(email, cancellationToken))
            throw new InvalidOperationException("A pending company registration already exists for this email.");

        var registration = new CompanyRegistration
        {
            CompanyName = request.CompanyName.Trim(), Website = request.Website?.Trim(),
            Address = request.Address?.Trim(), Industry = request.Industry?.Trim(),
            Description = request.Description?.Trim(), EmployeeCount = request.EmployeeCount,
            ContactFullName = request.ContactFullName.Trim(), ContactEmail = email,
            ContactPhoneNumber = request.ContactPhoneNumber?.Trim()
        };
        await _companies.AddRegistrationAsync(registration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return registration.ToResponse();
    }
}
