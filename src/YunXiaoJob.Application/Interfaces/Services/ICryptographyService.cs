namespace YunXiaoJob.Application.Interfaces.Services;

public interface ICryptographyService
{
    string ComputeHash(string value);
    string GenerateSecureToken(int byteLength = 32);
}
