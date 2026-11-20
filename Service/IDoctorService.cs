using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;

namespace Service
{
    public interface IDoctorService
    {
        Task<GeneralResponse<AddDoctorResponseDto>> AddDoctor(AddDoctorRequestDto request);
    }
}
