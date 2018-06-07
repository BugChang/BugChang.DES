using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Departments
{
    public interface IDepartmentRepository : IBasePageSearchRepository<Department>
    {
        Task<IList<Department>> GetAllAsync(int? parentId);

        Task<Department> GetAsync(string code, int? parentId);

        Task<int> GetCountAsync(int parentId);
        
    }
}
