using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Users;

namespace BugChang.DES.Core.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> Get(int id);

        Task<IList<TEntity>> GetAll();

        Task AddAsync(TEntity entity);

        Task Delete(int id);

        Task Update(TEntity entity);
    }
}
