using Azure.Core;
using Library.Core.Entities.Identity;
using Library.Core.ServiceContract;
using LibraTrack.API.DTOs;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace LibraTrack.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(IAuthService authService, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));
            var accessToken = await _authService.CreateTokenAsync(user, _userManager);
            var refreshToken = _authService.GenerateRefreshToken();

            await _authService.SaveRefreshTokenAsync(user, refreshToken);
            return Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.Fullname,
                Token = await _authService.CreateTokenAsync(user, _userManager),
                Roles = (List<string>)await _userManager.GetRolesAsync(user),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            });
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExist(model.Email).Result.Value)
            {
                return BadRequest(new ApiValidationErrorResponse()
                {
                    Errors = new[] { "Email is already in use" }
                });
            }
            var user = new AppUser
            {
                Fullname = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(404));
            var accessToken = await _authService.CreateTokenAsync(user, _userManager);
            var refreshToken = _authService.GenerateRefreshToken();

            await _authService.SaveRefreshTokenAsync(user, refreshToken);
            await _userManager.AddToRoleAsync(user, "Member");

            return Ok(
                new UserDto
                {
                    Email = user.Email,
                    DisplayName = user.Fullname,
                    Token = await _authService.CreateTokenAsync(user, _userManager),
                    Roles = (List<string>)await _userManager.GetRolesAsync(user),
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                }
                );
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto model)
        {
            var token = model.RefreshToken;

            var refreshToken = await _authService.GetRefreshTokenAsync(token);
            if (refreshToken == null || refreshToken.RevokedOn != null || refreshToken.ExpiresOn < DateTime.UtcNow)
                return Unauthorized();

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _authService.RevokeRefreshTokenAsync(refreshToken);

            return Ok(new { message = "Logout successful" });
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (user == null)
                return NotFound(new ApiResponse(404));
            return Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.Fullname,
                Token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""),
            });
        }
        [HttpGet("emailExist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound(new ApiResponse(404, "User Not Found"));
            user.Fullname = model.Fullname;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to update user information"));
            return Ok(new
            {
                message = "User information updated successfully",
                user = new
                {
                    user.Fullname,
                    user.Email,
                    user.PhoneNumber
                }
            });
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound(new ApiResponse(404, "User Not Found"));
            await _authService.DeleteRefreshTokensForUserAsync(user.Id);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to delete user account"));
            return Ok(new { message = "User account deleted successfully" });
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var refreshToken = await _authService.GetRefreshTokenAsync(model.RefreshToken);
            if (refreshToken is null || refreshToken.ExpiresOn < DateTime.UtcNow || refreshToken.RevokedOn is not null)
                return Unauthorized(new ApiResponse(401, "Invalid or expired refresh token"));
            var user = await _userManager.FindByIdAsync(refreshToken.UserId);

            var newAccessToken = await _authService.CreateTokenAsync(user, _userManager);

            var newRefreshToken = _authService.GenerateRefreshToken();
            await _authService.SaveRefreshTokenAsync(user, newRefreshToken);
            return Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.Fullname,
                Token = newAccessToken,
                Roles = (List<string>)await _userManager.GetRolesAsync(user),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            });
        }
        [HttpPut("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var CheckConfirmationEmail = await _userManager.IsEmailConfirmedAsync(user);
            if (user is null || !CheckConfirmationEmail)
            {
                return Ok(new { message = "If the email is registered and confirmed, a reset link has been sent." });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetlink = Url.Action("ResetPassword", "Action", new
            {
                Token = token,
                model.Email,
                Request.Scheme
            });
            // TODO: Send 'resetlink' to email
            Console.WriteLine(resetlink); // For testing

            return Ok(new { message = "Reset link sent to your email." });
        }
        [HttpPost("Reset-Password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest(new { Message = "Invalid Request" });
            var Result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!Result.Succeeded)
            {
                var errors = Result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Reset failed", errors });
            }
            return Ok(new { Message = "Password reset Successfully." });
        }
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound(new ApiResponse(404, "User Not Found"));
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to change password"));
            return Ok(new { message = "Password changed successfully" });
        }
    }
}
