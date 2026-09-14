using System.Security.Cryptography;
using System.Text;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Infrastructure.Settings;

namespace YunXiaoJob.Infrastructure.Services;

public class CryptographyService : ICryptographyService
{
    private const int KeyLength = 32;
    private const int NonceLength = 12;
    private const int TagLength = 16;
    private readonly byte[] _encryptionKey;

    public CryptographyService(CryptographySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            _encryptionKey = Convert.FromBase64String(settings.EncryptionKey);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("Cryptography encryption key must be Base64 encoded.", exception);
        }

        if (_encryptionKey.Length != KeyLength)
            throw new InvalidOperationException("Cryptography encryption key must contain exactly 32 bytes.");
    }

    public string Encrypt(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        var nonce = RandomNumberGenerator.GetBytes(NonceLength);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherText = new byte[plainTextBytes.Length];
        var tag = new byte[TagLength];

        using var aes = new AesGcm(_encryptionKey, TagLength);
        aes.Encrypt(nonce, plainTextBytes, cipherText, tag);

        var payload = new byte[NonceLength + TagLength + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceLength);
        Buffer.BlockCopy(tag, 0, payload, NonceLength, TagLength);
        Buffer.BlockCopy(cipherText, 0, payload, NonceLength + TagLength, cipherText.Length);
        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string cipherText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText);

        byte[] payload;
        try
        {
            payload = Convert.FromBase64String(cipherText);
        }
        catch (FormatException exception)
        {
            throw new CryptographicException("Cipher text is not valid Base64.", exception);
        }

        if (payload.Length < NonceLength + TagLength)
            throw new CryptographicException("Cipher text payload is invalid.");

        var nonce = payload[..NonceLength];
        var tag = payload[NonceLength..(NonceLength + TagLength)];
        var encrypted = payload[(NonceLength + TagLength)..];
        var plainText = new byte[encrypted.Length];

        using var aes = new AesGcm(_encryptionKey, TagLength);
        aes.Decrypt(nonce, encrypted, tag, plainText);
        return Encoding.UTF8.GetString(plainText);
    }

    public string ComputeHash(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    public bool VerifyHash(string value, string hash)
    {
        if (value is null || string.IsNullOrWhiteSpace(hash)) return false;

        try
        {
            var expectedHash = Convert.FromBase64String(hash);
            var actualHash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public string GenerateSecureToken(int byteLength = 32)
    {
        if (byteLength < 16) throw new ArgumentOutOfRangeException(nameof(byteLength), "Token must contain at least 16 bytes.");
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength));
    }
}
