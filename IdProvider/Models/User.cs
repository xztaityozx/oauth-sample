using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IdProvider.Models
{
    public record User(int Id, string Name, byte[] Hashed, byte[] Salt)
    {
        private const int IterationOfHash = 1000;
        public static User GenerateUser(int id, string name, string password)
        {
            var salt = new byte[128 / 8];
            using var rand = RandomNumberGenerator.Create();
            rand.GetBytes(salt);
            var hashed = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, IterationOfHash, 256 / 8);

            return new User(id, name, hashed, salt);
        }

        public bool Login(string password)
        {
            var hashed = KeyDerivation.Pbkdf2(password, Salt, KeyDerivationPrf.HMACSHA256, IterationOfHash, 256 / 8);

            return hashed.SequenceEqual(Hashed);
        }
    };

    public record UserCreateRequest([Required] string Name, [Required] string Password);

    public record UserDeleteRequest([Required] int Id, [Required] string Password);

    public record UserResponse(int Id, string Name);
}
