using System.Threading.Tasks;
using BugChang.DES.Domain.Services.Accounts;
using BugChang.DES.EntityFrameWorkCore;
using BugChang.DES.Infrastructure;

namespace BugChang.DES.Application.Accounts
{
    public class AccountAppService : IAccountAppService
    {
        private readonly IAccountServcice _accountServcice;
        private readonly UnitOfWork<MainDbContext> _mainUnitOfWork;

        public AccountAppService(IAccountServcice accountServcice, UnitOfWork<MainDbContext> mainUnitOfWork)
        {
            _accountServcice = accountServcice;
            _mainUnitOfWork = mainUnitOfWork;
        }

        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            var loginResult = await _accountServcice.LoginAysnc(userName, password);
            await _mainUnitOfWork.CommitAsync();
            return loginResult;
        }
    }
}
