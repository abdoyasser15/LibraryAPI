using Library.Core.Entities.Dtos;
using Library.Core.Entities.Identity;
using Library.Core.ServiceContract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager) 
        {
            _userManager = userManager;
        }



        public async Task<IReadOnlyList<UserDto>> GetAllUserAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "Unknown",
                    CreatedAt = user.CreatedAt
                });
            }
            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            var roles =await _userManager.GetRolesAsync(user);
            var result = new UserDto
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "Unknown",
                CreatedAt = user.CreatedAt
            };
            return result;
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var currentRole = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRole);
            var result = await _userManager.AddToRoleAsync(user, model.NewRole);
            return result.Succeeded;
        }
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.Users.Where(U=>U.Id==id).FirstOrDefaultAsync();
            if (user == null) return false;
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return false;
            return result.Succeeded;
        }
    }
}
