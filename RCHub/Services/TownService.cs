using Microsoft.EntityFrameworkCore;
using RCHub.Data;
using RCHub.Models.Entities;
using RCHub.Services.Base;

namespace RCHub.Services
{
    public class TownService : BaseService<Town>
    {
        public TownService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        public async Task<Town> FindByNameAsync(string name)
        {
            return await Query.FirstOrDefaultAsync(q => q.Name == name);
        }

        public async Task<List<Town>> GetByNamesAsync(List<string> names)
        {
            return await Query.Where(q => names.Any(n => q.Name == n)).ToListAsync();
        }
    }
}
