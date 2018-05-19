using System.Threading.Tasks;
using BugChang.DES.Infrastructure;

namespace BugChang.DES.Domain.Services.Accounts
{
    public interface IAccountServcice
    {
        Task<LoginResult> LoginAysnc(string userName, string password);
    }
}
