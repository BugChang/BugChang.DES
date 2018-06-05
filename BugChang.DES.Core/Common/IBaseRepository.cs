using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugChang.DES.Core.Common
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetAsync(int id);

        Task<IList<TEntity>> GetAllAsync();

        IQueryable<TEntity> GetQueryable();

        Task AddAsync(TEntity entity);

        Task DeleteAsync(int id);

        void Update(TEntity entity);
    }
}
