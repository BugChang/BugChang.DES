using System.Threading.Tasks;
using BugChang.DES.Core.Authentication;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Accounts
{
    public class AccountAppService : IAccountAppService
    {
        private readonly LoginManager _loginManager;
        private readonly UnitOfWork _mainUnitOfWork;

        public AccountAppService(UnitOfWork mainUnitOfWork, LoginManager loginManager)
        {
            _mainUnitOfWork = mainUnitOfWork;
            _loginManager = loginManager;
        }

        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            var loginResult = await _loginManager.LoginAysnc(userName, password);
            await _mainUnitOfWork.CommitAsync();
            return loginResult;
        }
    }
}
