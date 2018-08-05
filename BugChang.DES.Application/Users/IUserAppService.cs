using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Users
{
    public interface IUserAppService : ICurdAppService<UserEditDto, UserListDto>
    {
        Task<IList<UserListDto>> GetUsersAsync();

        Task<IList<RoleListDto>> GetUserRoles(int userId);

        Task<ResultEntity> AddUserRole(int userId, int roleId, int operatorId);

        Task<ResultEntity> DeleteUserRole(int userId, int userRole, int operatorId);

        Task<ResultEntity> ChangeUserEnabled(int userId, int operatorId);
    }
}
