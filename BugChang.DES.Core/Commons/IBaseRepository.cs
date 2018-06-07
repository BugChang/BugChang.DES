using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugChang.DES.Core.Commons
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IList<TEntity>> GetAllAsync();

        IQueryable<TEntity> GetQueryable();

        Task AddAsync(TEntity entity);

        Task DeleteByIdAsync(int id);

        void Update(TEntity entity);
    }

    public interface IBasePageSearchRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<PageResultModel<TEntity>> GetPagingAysnc(PageSearchModel pageSearchModel);
    }
}
