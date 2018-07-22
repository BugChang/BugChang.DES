using System.Threading.Tasks;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Accounts
{
    public interface IAccountAppService
    {
        Task<LoginResult> LoginAsync(string userName, string password);

        Task<ResultEntity> ChangePassword(int userId, string password, string oldPassword);
    }
}
