using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _db;
        private DbSet<TEntity> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? filter = null, bool isTracked = true, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<TEntity> query = dbSet;

            if (!isTracked)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if(pageSize > 0)
            {
                if (pageSize > 100)
                    pageSize = 100;

                //page number 1 && page size 5
                //5 * (1-1)
                //skip 0 take 5

                //page number 2 && page size 5
                //5 * (2-1)
                //skip 1 take 5
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if(includeProperties != null)
            {
                foreach(var include in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (pageSize > 0)
            {
                if (pageSize > 100)
                    pageSize = 100;

                //page number 1 && page size 5
                //5 * (1-1)
                //skip 0 take 5

                //page number 2 && page size 5
                //5 * (2-1)
                //skip 1 take 5
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (includeProperties != null)
            {
                foreach (var include in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
