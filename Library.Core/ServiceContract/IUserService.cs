using Library.Core.Entities.Dtos;
using Library.Core.Entities.Identity;
using Library.Core.RepositoryContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface IUserService 
    {
        Task<IReadOnlyList<UserDto>> GetAllUserAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto model);
        Task<bool> DeleteUserAsync(string id);
    }
}
