using System.Threading.Tasks;
using BugChang.DES.Core.Authentication;

namespace BugChang.DES.Application.Accounts
{
    public interface IAccountAppService
    {
        Task<LoginResult> LoginAsync(string userName, string password); 
    }
}
