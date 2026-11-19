

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital_Service.Models;
namespace CommonLayer.SecurityHelper
{
    public interface ISecurityHelper
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        (string Token, DateTime ExpiresAtUtc) GenerateJwtToken(User user);
    }

}
