using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.CompanyRegistrations;
namespace YunXiaoJob.API.Controllers;
[Route("api/company-registrations")]
public class CompanyRegistrationsController : ApiControllerBase
{
    [HttpPost] public async Task<IActionResult> Submit(SubmitCompanyRegistrationRequest request, [FromServices] SubmitCompanyRegistrationUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, ct));
}
