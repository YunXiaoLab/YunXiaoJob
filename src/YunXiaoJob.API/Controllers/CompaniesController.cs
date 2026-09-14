using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Companies;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/companies")]
public class CompaniesController : ApiControllerBase
{
    [HttpPost] public async Task<IActionResult> Create(CreateCompanyRequest request, [FromServices] CreateCompanyUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateCompanyRequest request, [FromServices] UpdateCompanyUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("{id:guid}/members")] public async Task<IActionResult> AddMember(Guid id, AddCompanyMemberRequest request, [FromServices] AddCompanyMemberUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
}
