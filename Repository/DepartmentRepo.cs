using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Request;
using Hospital_Service.Data;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public class DepartmentRepo : IDepartmentRepo
    {
        private readonly HospitalDbContext _context;

        public DepartmentRepo(HospitalDbContext context)
        {
            _context = context;
        }

        // Includes deleted rows, because the unique keys cover every row
        public async Task<Department?> GetByNameAsync(string name)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<Department?> GetByCodeAsync(string code)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task AddDepartmentAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        // DepartmentRepo
        public async Task<List<DepartmentListItemDto>> GetDepartmentListAsync()
        {
            return await _context.Departments.AsNoTracking()
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentListItemDto
                {
                    DepartmentId = d.Id,
                    Name = d.Name,
                    Code = d.Code,
                    HeadDoctorId = d.HeadDoctorId,
                    HeadDoctorName = d.HeadDoctor != null ? d.HeadDoctor.User.FullName : null,
                    DoctorCount = d.Doctors.Count(doc => !doc.IsDeleted),
                    IsActive = d.IsActive ?? true
                })
                .ToListAsync();
        }
    }
}
