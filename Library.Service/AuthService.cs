using Library.Core.Entities.Identity;
using Library.Core.ServiceContract;
using Library.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Library.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppIdentityDbContext _dbContext;

        public AuthService(IConfiguration configuration , 
            UserManager<AppUser> userManager , AppIdentityDbContext dbContext)
        {
            _configuration = configuration;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager)
        {
            
            // Private Claims (User-Defined)
            var authClaims = new List<Claim>()
            {
                new Claim("Fullname", User.Fullname),
                new Claim(ClaimTypes.Email,User.Email),
                new Claim(ClaimTypes.NameIdentifier,User.Id),
            };
            var userRoles = await userManager.GetRolesAsync(User);
            foreach (var role in userRoles) {
                authClaims.Add(new Claim(ClaimTypes.Role,role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:DurationInDays"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var reandomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(reandomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(reandomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task SaveRefreshTokenAsync(AppUser user, RefreshToken refreshToken)
        {
            refreshToken.UserId = user.Id;
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteRefreshTokensForUserAsync(string userId)
        {
            var tokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();

            _dbContext.RefreshTokens.RemoveRange(tokens);
            await _dbContext.SaveChangesAsync();
        }
    }
}
