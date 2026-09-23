using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Application.UseCases.CompanyRegistrations;

internal static class CompanyRegistrationMapping
{
    public static CompanyRegistrationResponse ToResponse(this CompanyRegistration x) =>
        new(x.Id, x.CompanyName, x.Website, x.Address, x.Industry, x.Description, x.EmployeeCount,
            x.ContactFullName, x.ContactEmail, x.ContactPhoneNumber, x.Status, x.ReviewNote, x.ReviewedAtUtc,
            x.CreatedCompanyId, x.CreatedOwnerUserId, x.CreatedAtUtc);
}
