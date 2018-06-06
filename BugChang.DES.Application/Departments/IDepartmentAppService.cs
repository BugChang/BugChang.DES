using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Application.Departments
{
    public interface IDepartmentAppService : ICurdAppService<DepartmentEditDto>
    {
        Task<IList<DepartmentDto>> GetAllAsync(int? parentId);

        Task<IList<DepartmentDto>> GetAllAsync();

        Task<PageResultEntity<DepartmentDto>> GetPagingAysnc(int? parentId, int take, int skip, string keywords);
    }
}
