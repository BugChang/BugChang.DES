using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.IRepository;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {

        public Task<TEntity> Get(int id)
        {
            var isInstance = typeof(TEntity).IsInstanceOfType(typeof(BaseEntity));
            throw new NotImplementedException();
        }

        public Task<IList<TEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(TEntity entity)
        {
            using (var dbContext = new BasicDdContext())
            {
                await dbContext.Users.AddAsync(entity as User);
            }
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
