using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;

namespace Service
{
    public interface IDepartmentService
    {
        Task<GeneralResponse<DepartmentResponseDto>> AddDepartment(AddDepartmentRequestDto request);
        // IDepartmentService
        Task<GeneralResponse<List<DepartmentListItemDto>>> GetDepartmentList();

    }
}
