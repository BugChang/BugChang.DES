using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Users;

namespace BugChang.DES.Application.UserApp
{
    public interface IUserAppService
    {
        Task<bool> CheckAsync(string userName, string password);

        Task AddAsync(User user);
    }
}
