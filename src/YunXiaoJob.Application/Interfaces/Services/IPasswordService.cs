namespace YunXiaoJob.Application.Interfaces.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    string GenerateTemporaryPassword(int length = 12);
}
