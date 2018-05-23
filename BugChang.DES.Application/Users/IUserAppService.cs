using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Users.Dtos;

namespace BugChang.DES.Application.Users
{
    public interface IUserAppService
    {
        Task<IList<UserDto>> GetUsersAsync();
    }
}
