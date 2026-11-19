using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using CommonLayer.SecurityHelper;
using Dto.Request;
using Entities.Models.Enum;
using Hospital_Service.Models;
using Microsoft.Extensions.Configuration;
using Repository;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;
        private readonly ISecurityHelper _securityHelper;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepo authRepo, ISecurityHelper securityHelper, IConfiguration config)
        {
            _authRepo = authRepo;
            _securityHelper = securityHelper;
            _config = config;
        }

        public async Task<GeneralResponse<LoginResponseDto>> LoginUser(LoginRequestDto request)
        {
            var login = request.MobileOrEmail.Trim();
            var user = await _authRepo.GetUserForLoginAsync(login);

            // First login of the Owner: create the Owner from appsettings
            if (user == null && await TryCreateOwnerFromSettingsAsync(login, request.Password))
            {
                user = await _authRepo.GetUserForLoginAsync(login);
            }

            if (user == null || !_securityHelper.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new GeneralResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Invalid mobile/email or password",
                    HttpStatusCode = HttpStatusCode.Unauthorized,
                    Data = null
                };
            }

            if (!Enum.IsDefined(typeof(UserRole), user.Role))
            {
                return new GeneralResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Your account role is not set up correctly. Please contact the hospital.",
                    HttpStatusCode = HttpStatusCode.Forbidden,
                    Data = null
                };
            }

            var role = (UserRole)user.Role;
            var (token, expiresAtUtc) = _securityHelper.GenerateJwtToken(user);

            var loginResponse = new LoginResponseDto
            {
                UserId = user.Id,
                HospitalName = _config["Setup:Hospital:Name"],
                FullName = user.FullName,
                Role = (byte)user.Role,
                RoleName = role.ToString(),
                DoctorId = user.Doctor?.Id,
                DepartmentId = user.Doctor?.DepartmentId,
                PatientId = user.PatientUser?.Id,
                PhotoPath = user.PhotoPath,
                MustChangePassword = user.MustChangePassword,
                Token = token,
                ExpiresAtUtc = expiresAtUtc
            };

            return new GeneralResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful",
                HttpStatusCode = HttpStatusCode.OK,
                Data = loginResponse
            };
        }

        public async Task<BaseResponse> ChangePassword(long userId, ChangePasswordRequestDto request)
        {
            var user = await _authRepo.GetUserByIdAsync(userId);

            if (user == null || !_securityHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Current password is incorrect",
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            if (_securityHelper.VerifyPassword(request.NewPassword, user.PasswordHash))
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "New password must be different from the current password",
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            user.PasswordHash = _securityHelper.HashPassword(request.NewPassword);
            user.MustChangePassword = false;
            await _authRepo.UpdateUserAsync(user);

            return new BaseResponse
            {
                Success = true,
                Message = "Password changed successfully",
                HttpStatusCode = HttpStatusCode.OK
            };
        }

        // Creates the Owner only when no Owner exists yet and the credentials
        // match the Setup:Owner section. Returns true when created.
        private async Task<bool> TryCreateOwnerFromSettingsAsync(string login, string password)
        {
            var ownerMobile = _config["Setup:Owner:Mobile"];
            var defaultPassword = _config["Setup:Owner:DefaultPassword"];

            if (string.IsNullOrWhiteSpace(ownerMobile) || string.IsNullOrWhiteSpace(defaultPassword))
                return false;
            if (login != ownerMobile.Trim())
                return false;
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(defaultPassword)))
                return false;
            if (await _authRepo.OwnerExistsAsync())
                return false;

            await _authRepo.AddUserAsync(new User
            {
                FullName = _config["Setup:Owner:FullName"] ?? "Owner",
                Mobile = ownerMobile.Trim(),
                Email = _config["Setup:Owner:Email"],
                PasswordHash = _securityHelper.HashPassword(defaultPassword),
                MustChangePassword = true,
                Role = UserRole.Owner,
                IsActive = true
            });

            return true;
        }
    }
}
