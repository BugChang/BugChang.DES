using System.Threading.Tasks;
using BugChang.DES.Domain.Services.Accounts;
using BugChang.DES.Infrastructure;

namespace BugChang.DES.Application.Accounts
{
    public class AccountAppService : IAccountAppService
    {
        private readonly IAccountServcice _accountServcice;


        public AccountAppService(IAccountServcice accountServcice)
        {
            _accountServcice = accountServcice;
        }

        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            return await _accountServcice.LoginAysnc(userName, password);
        }
    }
}
