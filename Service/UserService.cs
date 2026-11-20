using System.Net;
using CommonLayer.CommonResponse;
using CommonLayer.SecurityHelper;
using Dto.Request;
using Dto.Response;
using Entities.Models.Enum;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;

namespace Service
{
    public class UserService : IUserService
    {
        private static readonly UserRole[] AllowedRoles =
            { UserRole.DeptHead, UserRole.Doctor, UserRole.Assistant, UserRole.Patient };

        private readonly IUserRepo _userRepo;
        private readonly ISecurityHelper _securityHelper;
        private readonly IConfiguration _config;

        public UserService(IUserRepo userRepo, ISecurityHelper securityHelper, IConfiguration config)
        {
            _userRepo = userRepo;
            _securityHelper = securityHelper;
            _config = config;
        }

        public async Task<GeneralResponse<AddUserResponseDto>> AddUser(AddUserRequestDto request)
        {
            var role = (UserRole)request.Role;
            if (!AllowedRoles.Contains(role))
            {
                return Fail("This role cannot be added.", HttpStatusCode.BadRequest);
            }

            var defaultPassword = _config["Setup:Owner:DefaultUserPassword"];
            if (string.IsNullOrWhiteSpace(defaultPassword))
            {
                return Fail("Default user password is not configured on the server.", HttpStatusCode.InternalServerError);
            }

            var mobile = request.Mobile.Trim();
            var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            if (await _userRepo.MobileExistsAsync(mobile))
            {
                return Fail("A user with this mobile number already exists.", HttpStatusCode.Conflict);
            }

            if (email != null && await _userRepo.EmailExistsAsync(email))
            {
                return Fail("A user with this email already exists.", HttpStatusCode.Conflict);
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Mobile = mobile,
                Email = email,
                PasswordHash = _securityHelper.HashPassword(defaultPassword),
                MustChangePassword = true,
                Role = role,
                IsActive = true
            };

            try
            {
                await _userRepo.AddUserAsync(user);
            }
            catch (DbUpdateException)
            {
                return Fail("A user with this mobile number already exists.", HttpStatusCode.Conflict);
            }

            return new GeneralResponse<AddUserResponseDto>
            {
                Success = true,
                Message = "User added. They can log in with the default password and must change it on first login.",
                HttpStatusCode = HttpStatusCode.Created,
                Data = new AddUserResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    Role = (byte)user.Role,
                    RoleName = user.Role.ToString(),
                    MustChangePassword = true
                }
            };
        }

        private static GeneralResponse<AddUserResponseDto> Fail(string message, HttpStatusCode status)
        {
            return new GeneralResponse<AddUserResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = status,
                Data = null
            };
        }

        public async Task<GeneralResponse<PagedResponseDto<StaffListItemDto>>> GetStaffList(StaffListQueryDto query)
        {
            var (items, total) = await _userRepo.GetStaffListAsync(query);

            foreach (var item in items)
            {
                item.RoleName = ((UserRole)item.Role).ToString();
            }

            return new GeneralResponse<PagedResponseDto<StaffListItemDto>>
            {
                Success = true,
                Message = items.Count == 0 ? "No records found." : "Success",
                HttpStatusCode = HttpStatusCode.OK,
                Data = new PagedResponseDto<StaffListItemDto>
                {
                    Items = items,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = total
                }
            };
        }

        public async Task<GeneralResponse<StaffDetailDto>> GetStaffDetail(StaffDetailQueryDto query)
        {
            var staff = await _userRepo.GetStaffDetailAsync(query.Id);
            if (staff == null)
            {
                return new GeneralResponse<StaffDetailDto>
                {
                    Success = false,
                    Message = "User not found.",
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            // The role passed must match the user's real role
            if (staff.Role != query.Role)
            {
                return new GeneralResponse<StaffDetailDto>
                {
                    Success = false,
                    Message = $"User {query.Id} is a {((UserRole)staff.Role)}, not a {((UserRole)query.Role)}.",
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            staff.RoleName = ((UserRole)staff.Role).ToString();

            return new GeneralResponse<StaffDetailDto>
            {
                Success = true,
                Message = "Success",
                HttpStatusCode = HttpStatusCode.OK,
                Data = staff
            };
        }
    }
    }