using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepo _departmentRepo;

        public DepartmentService(IDepartmentRepo departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        public async Task<GeneralResponse<DepartmentResponseDto>> AddDepartment(AddDepartmentRequestDto request)
        {
            var name = request.Name.Trim();
            var code = request.Code.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(name))
            {
                return Fail("Department name is required.", HttpStatusCode.BadRequest);
            }

            var byName = await _departmentRepo.GetByNameAsync(name);
            if (byName != null)
            {
                return Fail(byName.IsDeleted
                    ? $"{name} was deleted earlier. Restore it instead of creating it again."
                    : $"{name} already exists.", HttpStatusCode.Conflict);
            }

            var byCode = await _departmentRepo.GetByCodeAsync(code);
            if (byCode != null)
            {
                return Fail($"Code {code} is already used by {byCode.Name}.", HttpStatusCode.Conflict);
            }

            var department = new Department
            {
                Name = name,
                Code = code,
                HeadDoctorId = null,   // head is set later from the Doctor API
                IsActive = true
            };

            try
            {
                await _departmentRepo.AddDepartmentAsync(department);
            }
            catch (DbUpdateException)
            {
                return Fail("A department with this name or code already exists.", HttpStatusCode.Conflict);
            }

            return new GeneralResponse<DepartmentResponseDto>
            {
                Success = true,
                Message = "Department added.",
                HttpStatusCode = HttpStatusCode.Created,
                Data = new DepartmentResponseDto
                {
                    DepartmentId = department.Id,
                    Name = department.Name,
                    Code = code,
                    HeadDoctorId = null,
                    IsActive = true
                }
            };
        }

        private static GeneralResponse<DepartmentResponseDto> Fail(string message, HttpStatusCode status)
        {
            return new GeneralResponse<DepartmentResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = status,
                Data = null
            };
        }

     
        // DepartmentService
        public async Task<GeneralResponse<List<DepartmentListItemDto>>> GetDepartmentList()
        {
            var list = await _departmentRepo.GetDepartmentListAsync();
            return new GeneralResponse<List<DepartmentListItemDto>>
            {
                Success = true,
                Message = "Success",
                HttpStatusCode = HttpStatusCode.OK,
                Data = list
            };
        }
    }
}
