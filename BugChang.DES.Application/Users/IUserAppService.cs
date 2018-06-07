using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Users.Dtos;

namespace BugChang.DES.Application.Users
{
    public interface IUserAppService : ICurdAppService<UserEditDto,UserListDto>
    {
        Task<IList<UserListDto>> GetUsersAsync();
    }
}
