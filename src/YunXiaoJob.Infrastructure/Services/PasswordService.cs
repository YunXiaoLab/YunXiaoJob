using System.Security.Cryptography;
using YunXiaoJob.Application.Interfaces.Services;

namespace YunXiaoJob.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private const int Iterations = 210_000;
    private const int SaltLength = 16;
    private const int HashLength = 32;

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashLength);
        return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public string GenerateTemporaryPassword(int length = 12)
    {
        if (length < 8) throw new ArgumentOutOfRangeException(nameof(length), "A temporary password needs at least 8 characters.");
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string symbols = "!@#$%&*?";
        var alphabet = upper + lower + digits + symbols;
        var characters = new char[length];
        characters[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        characters[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        characters[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        characters[3] = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];
        for (var i = 4; i < length; i++) characters[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
        for (var i = characters.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[j]) = (characters[j], characters[i]);
        }
        return new string(characters);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash)) return false;
        var parts = passwordHash.Split('.');
        if (parts.Length != 4 || parts[0] != "v1" || !int.TryParse(parts[1], out var iterations) ||
            iterations < 100_000 || iterations > 1_000_000) return false;

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expectedHash = Convert.FromBase64String(parts[3]);
            if (salt.Length != SaltLength || expectedHash.Length != HashLength) return false;

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512,
                HashLength);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
