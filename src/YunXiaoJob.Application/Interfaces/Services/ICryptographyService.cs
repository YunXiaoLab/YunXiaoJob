namespace YunXiaoJob.Application.Interfaces.Services;

public interface ICryptographyService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string ComputeHash(string value);
    bool VerifyHash(string value, string hash);
    string GenerateSecureToken(int byteLength = 32);
}
