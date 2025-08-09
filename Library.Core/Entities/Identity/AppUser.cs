using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Library.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string Fullname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
