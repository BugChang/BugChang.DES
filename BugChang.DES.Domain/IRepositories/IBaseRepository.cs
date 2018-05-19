using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Domain.IRepositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetAsync(int id);

        Task<IList<TEntity>> GetAllAsync();

        Task AddAsync(TEntity entity);

        Task DeleteAsync(int id);

        Task UpdateAsync(TEntity entity);
    }
}
