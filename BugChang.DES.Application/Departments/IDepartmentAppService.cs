using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Departments.Dtos;

namespace BugChang.DES.Application.Departments
{
    public interface IDepartmentAppService : ICurdAppService<DepartmentEditDto,DepartmentListDto>
    {
        Task<IList<DepartmentListDto>> GetAllAsync(int? parentId);

        Task<IList<DepartmentListDto>> GetAllAsync();

        Task<DepartmentEditDto> GetDepartmentByCode(string code);

        Task<int> CheckForImport(int parentId, string code, string name);

    }
}
