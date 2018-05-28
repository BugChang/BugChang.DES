using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Departments
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<IList<Department>> GetAllAsync(int? parentId);

        Task<PageResultEntity<Department>> GetPagingAysnc(int? parentId, int take, int skip);

        Task<Department> GetAsync(string code, int? parentId);
    }
}
