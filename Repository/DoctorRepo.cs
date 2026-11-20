using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital_Service.Data;
using Hospital_Service.Models;

using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public class DoctorRepo : IDoctorRepo
    {
        private readonly HospitalDbContext _context;

        public DoctorRepo(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetActiveUserAsync(long userId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive != false);
        }

        // Includes deleted profiles, because doctors.user_id is unique for every row
        public async Task<bool> DoctorProfileExistsAsync(long userId)
        {
            return await _context.Doctors.AnyAsync(d => d.UserId == userId);
        }

        // Includes deleted doctors, because the unique key on code covers every row
        public async Task<Doctor?> GetDoctorByCodeAsync(string code)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        // Tracked, so the department head can be updated in the same save
        public async Task<Department?> GetActiveDepartmentAsync(long departmentId)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == departmentId && !d.IsDeleted && d.IsActive != false);
        }

        public async Task<Doctor?> GetActiveDoctorAsync(long doctorId)
        {
            return await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted);
        }

        // One SaveChanges = one transaction: insert doctor, then set department head if needed
        public async Task AddDoctorAsync(Doctor doctor, Department department, bool makeHead)
        {
            _context.Doctors.Add(doctor);

            if (makeHead)
            {
                department.HeadDoctor = doctor;
            }

            await _context.SaveChangesAsync();
        }
    }
}
