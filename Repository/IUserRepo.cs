using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Request;
using Hospital_Service.Models;

namespace Repository
{
    public interface IUserRepo
    {
        Task<bool> MobileExistsAsync(string mobile);
        Task<bool> EmailExistsAsync(string email);
        Task AddUserAsync(User user);
        Task<(List<StaffListItemDto> Items, int Total)> GetStaffListAsync(StaffListQueryDto query);
        Task<StaffDetailDto?> GetStaffDetailAsync(long userId);
    }
}
