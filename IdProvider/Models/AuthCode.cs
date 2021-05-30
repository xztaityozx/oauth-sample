using System;

namespace IdProvider.Models
{
    public record AuthCode(int Id, string Code, int UserId)
    {
        public static AuthCode GenerateCode(int id, int userId) => new AuthCode(id, Guid.NewGuid().ToString("N"), userId);
    }

    public record GenerateAuthCodeRequest(int Id, int ServiceId);
}