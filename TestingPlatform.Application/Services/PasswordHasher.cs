
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TestingPlatform.Application.Interfaces;

namespace TestingPlatform.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Генерируем случайную соль
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Хешируем пароль
        byte[] hashed = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);

        // Объединяем соль и хеш
        byte[] hashBytes = new byte[salt.Length + hashed.Length];
        Array.Copy(salt, 0, hashBytes, 0, salt.Length);
        Array.Copy(hashed, 0, hashBytes, salt.Length, hashed.Length);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            byte[] hashBytes = Convert.FromBase64String(passwordHash);

            // Извлекаем соль
            byte[] salt = new byte[128 / 8];
            Array.Copy(hashBytes, 0, salt, 0, salt.Length);

            // Хешируем введенный пароль с той же солью
            byte[] expectedHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            // Сравниваем хеши
            for (int i = 0; i < expectedHash.Length; i++)
            {
                if (hashBytes[i + salt.Length] != expectedHash[i])
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}