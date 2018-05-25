using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Application.Departments
{
    public interface IDepartmentAppService
    {
        Task<IList<DepartmentDto>> GetAllAsync(int? parentId);

        Task<PageResultEntity<DepartmentDto>> GetPagingAysnc(int? parentId, int limit, int offset);
    }
}
