
using System;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Claims;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enum;
using Hospital_Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace CommonLayer.SecurityHelper
{
    public class SecurityHelper : ISecurityHelper
    {
        private readonly IConfiguration _config;

        public SecurityHelper(IConfiguration config)
        {
            _config = config;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public (string Token, DateTime ExpiresAtUtc) GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, ((UserRole)user.Role).ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            if (user.Doctor != null) claims.Add(new Claim("DoctorId", user.Doctor.Id.ToString()));

            var keyText = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyText) || keyText.Length < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var minutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 120;
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }

        public (string Token, DateTime ExpiresAtUtc) GenerateOwnerToken(string mobile)
        {
            var claims = new List<Claim>
    {
        new Claim("UserId", "0"),                 // Owner is not stored in the database
        new Claim("Mobile", mobile),
        new Claim(ClaimTypes.Role, UserRole.Owner.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            return CreateToken(claims);
        }

        private (string Token, DateTime ExpiresAtUtc) CreateToken(List<Claim> claims)
        {
            var keyText = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyText) || keyText.Length < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var minutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 120;
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }
    }

}
