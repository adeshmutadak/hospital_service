using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;
using Dto.Response;

namespace Service
{
    public interface IUserService
    {
        Task<GeneralResponse<AddUserResponseDto>> AddUser(AddUserRequestDto request);
        Task<GeneralResponse<PagedResponseDto<StaffListItemDto>>> GetStaffList(StaffListQueryDto query);
        Task<GeneralResponse<StaffDetailDto>> GetStaffDetail(StaffDetailQueryDto query);
    }
}
