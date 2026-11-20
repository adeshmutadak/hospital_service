using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Request;
using Hospital_Service.Models;

namespace Repository
{
    public interface IDepartmentRepo
    {
        Task<Department?> GetByNameAsync(string name);
        Task<Department?> GetByCodeAsync(string code);
        Task AddDepartmentAsync(Department department);
        // IDepartmentRepo
        Task<List<DepartmentListItemDto>> GetDepartmentListAsync();
    }
}
