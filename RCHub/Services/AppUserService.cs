using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using RCHub.Data;
using RCHub.Models.Entities;
using RCHub.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace RCHub.Services
{
    public class AppUserService : BaseService<AppUser>
    {
        public AppUserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        public async Task<AppUser?> FindByUsernameAsync(string username) => await Query.FirstOrDefaultAsync(u => u.Username == username);
        public async Task<AppUser?> FindByDiscordIdAsync(ulong discordId) => await Query.FirstOrDefaultAsync(u => u.DiscordId == discordId);

    }
}
