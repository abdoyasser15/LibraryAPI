using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraTrack.API.Controllers
{
    [ApiExplorerSettings(GroupName = "admin-v1")]
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUserAsync();
            if (users == null || !users.Any())
                return NotFound(new ApiResponse(404, "No users found."));
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"No user found with ID {id}.");
            return Ok(user);
        }
        [HttpPut("update-role")]
        public async Task<ActionResult> UpdateUserRoleAsync([FromBody] UpdateUserRoleDto model)
        {
            if (model is null || string.IsNullOrEmpty(model.UserId))
                return BadRequest(new ApiResponse(400, "Invalid user role update request."));
            var result = await _userService.UpdateUserRoleAsync(model);
            if (!result)
                return NotFound(new ApiResponse(404, "User not found or role update failed."));

            return Ok(new ApiResponse(200, "User role updated successfully."));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ApiResponse(400, "Invalid user ID."));
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound(new ApiResponse(404, "User not found or deletion failed."));
            return Ok(new ApiResponse(200, "User deleted successfully."));
        }
    }
}
