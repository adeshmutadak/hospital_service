using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital_Service.Models;

namespace Repository
{
    public interface IAuthRepo
    {
        Task<User?> GetUserForLoginAsync(string mobileOrEmail);
        Task<User?> GetUserByIdAsync(long userId);
        Task<bool> OwnerExistsAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
