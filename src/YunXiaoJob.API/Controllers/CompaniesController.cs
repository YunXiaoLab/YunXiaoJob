using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Companies;
using YunXiaoJob.Application.UseCases.Queries;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/companies")]
public class CompaniesController : ApiControllerBase
{
    [AllowAnonymous, HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id, [FromServices] GetCompanyProfileUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, ct));
    [HttpGet("mine")] public async Task<IActionResult> Mine([FromServices] GetMyCompaniesUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpGet("{id:guid}/members")] public async Task<IActionResult> Members(Guid id, [FromServices] GetCompanyMembersUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [HttpPost] public async Task<IActionResult> Create(CreateCompanyRequest request, [FromServices] CreateCompanyUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateCompanyRequest request, [FromServices] UpdateCompanyUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("{id:guid}/members")] public async Task<IActionResult> AddMember(Guid id, AddCompanyMemberRequest request, [FromServices] AddCompanyMemberUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("{id:guid}/staff")] public async Task<IActionResult> CreateStaff(Guid id, CreateCompanyStaffAccountRequest request, [FromServices] CreateCompanyStaffAccountUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
}
