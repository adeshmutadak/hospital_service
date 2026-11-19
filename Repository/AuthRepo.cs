using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enum;
using Hospital_Service.Data;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public class AuthRepo : IAuthRepo
    {
        private readonly HospitalDbContext _context;

        public AuthRepo(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserForLoginAsync(string mobileOrEmail)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Doctor)
                .Include(u => u.PatientUser)
                .FirstOrDefaultAsync(u => (u.Mobile == mobileOrEmail || u.Email == mobileOrEmail)
                                          && !u.IsDeleted
                                          && u.IsActive != false);
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive != false);
        }

        public async Task<bool> OwnerExistsAsync()
        {
            return await _context.Users.AnyAsync(u => u.Role == UserRole.Owner && !u.IsDeleted);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
