using YunXiaoJob.Domain.Enums;
namespace YunXiaoJob.Application.Interfaces.Services;
public interface IAuthTokenService { string CreateAccessToken(Guid userId, string email, PlatformRole platformRole); }
