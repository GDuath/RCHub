using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using RCHub.Data;
using RCHub.Models.Entities;

namespace RCHub.Services.Base
{
    public abstract class BaseService<TEntity> where TEntity : class, IEntity
    {
        protected readonly ApplicationDbContext _context;

        protected BaseService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
        }

        public virtual IQueryable<TEntity> Query => _context.Set<TEntity>();

        public virtual IList<TEntity> GetAll() => Query.ToList();
        public virtual async Task<List<TEntity>> GetAllAsync() =>  await Query.ToListAsync();
        public TEntity? Find(string id) => Query.FirstOrDefault(e => e.Id == id);
        public virtual Task<TEntity?> FindAsync(string id) => Query.FirstOrDefaultAsync(e => e.Id == id);

        public virtual EntityEntry<TEntity> Add(TEntity entity) => _context.Set<TEntity>().Add(entity);
        public virtual EntityEntry<TEntity> Update(TEntity entity) => _context.Update(entity);
        public virtual void Remove(TEntity entity) => _context.Set<TEntity>().Remove(entity);
        public virtual async Task RemoveAsync(string id)
        {
            var entity = await FindAsync(id);
            if (entity != null)
                Remove(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            var entries = _context.ChangeTracker.Entries();
            return _context.SaveChangesAsync();
        }

    }
}
