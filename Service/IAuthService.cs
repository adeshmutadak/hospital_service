using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;

namespace Service
{
    public interface IAuthService
    {
        Task<GeneralResponse<LoginResponseDto>> LoginUser(LoginRequestDto request);
        Task<BaseResponse> ChangePassword(long userId, ChangePasswordRequestDto request);
        Task<GeneralResponse<LoginResponseDto>> OwnerLogin(LoginRequestDto request);

    }
}
