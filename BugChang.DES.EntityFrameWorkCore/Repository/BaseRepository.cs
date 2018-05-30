using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DesDbContext _dbContext;

        protected BaseRepository(DesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> GetAsync(int id)
        {

            return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
    }
}
