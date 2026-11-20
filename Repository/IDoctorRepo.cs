using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital_Service.Models;

namespace Repository
{
    public interface IDoctorRepo
    {
        Task<User?> GetActiveUserAsync(long userId);
        Task<bool> DoctorProfileExistsAsync(long userId);
        Task<Doctor?> GetDoctorByCodeAsync(string code);
        Task<Department?> GetActiveDepartmentAsync(long departmentId);
        Task<Doctor?> GetActiveDoctorAsync(long doctorId);
        Task AddDoctorAsync(Doctor doctor, Department department, bool makeHead);
    }
}
