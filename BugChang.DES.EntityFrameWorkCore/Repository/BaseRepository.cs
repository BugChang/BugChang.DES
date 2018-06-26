using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity<int>
    {
        private readonly DesDbContext _dbContext;

        public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        protected BaseRepository(DesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await Table.SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return Table;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await Table.FindAsync(id);
            Table.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            Table.Update(entity);
        }
    }
}
