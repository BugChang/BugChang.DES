using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly MainDbContext _dbContext;

        protected BaseRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<TEntity> GetAsync(int id)
        {

            throw new NotImplementedException();
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

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
