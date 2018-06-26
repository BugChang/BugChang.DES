using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Roles
{
    public interface IRoleAppService : ICurdAppService<RoleEditDto, RoleListDto>
    {
        Task<ResultEntity> EditRoleMenu(int roleId, IList<int> lstMenuId);

        IList<string> GetRoleOperationCodes(string module, List<int> lstRoleId);


        Task<ResultEntity> AddRoleOperation(int roleId, string operationCode);

        Task<ResultEntity> DeleteRoleOperation(int roleId, string operationCode);

        Task<IList<RoleListDto>> GetAllRoles();
    }
}
