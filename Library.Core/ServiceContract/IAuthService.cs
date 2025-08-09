using Library.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(AppUser User , UserManager<AppUser> userManager);
        RefreshToken GenerateRefreshToken();
        Task SaveRefreshTokenAsync(AppUser user, RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
        Task DeleteRefreshTokensForUserAsync(string userId);
    }
}
