using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.API.Models;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Accounts;
namespace YunXiaoJob.API.Controllers;
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("register")] public async Task<IActionResult> Register(RegisterCandidateRequest request, [FromServices] StartRegistrationChallengeUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, ct));
    [HttpPost("register/verify")] public async Task<IActionResult> Verify(VerifyRegistrationOtpRequest request, [FromServices] VerifyRegistrationOtpUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, ct));
    [HttpPost("register/resend")] public async Task<IActionResult> Resend(ResendRegistrationOtpRequest request, [FromServices] ResendRegistrationOtpUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, ct));
    [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest request, [FromServices] LoginUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, ct));
    [HttpPost("refresh")] public async Task<IActionResult> Refresh(RefreshAccessTokenRequest request, [FromServices] RefreshAccessTokenUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, ct));
    [HttpPost("forgot-password")] public async Task<IActionResult> Forgot(ForgotPasswordRequest request, [FromServices] ForgotPasswordUseCase useCase, CancellationToken ct) { await useCase.ExecuteAsync(request, ct); return Ok(ApiResponse.Ok()); }
    [HttpPost("reset-password")] public async Task<IActionResult> Reset(ResetPasswordRequest request, [FromServices] ResetPasswordUseCase useCase, CancellationToken ct) { await useCase.ExecuteAsync(request, ct); return Ok(ApiResponse.Ok()); }
}
