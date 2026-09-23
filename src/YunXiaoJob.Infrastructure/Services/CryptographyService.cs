using System.Security.Cryptography;
using System.Text;
using YunXiaoJob.Application.Interfaces.Services;

namespace YunXiaoJob.Infrastructure.Services;

public class CryptographyService : ICryptographyService
{
    public string ComputeHash(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    public string GenerateSecureToken(int byteLength = 32)
    {
        if (byteLength < 16) throw new ArgumentOutOfRangeException(nameof(byteLength), "Token must contain at least 16 bytes.");
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength));
    }
}
