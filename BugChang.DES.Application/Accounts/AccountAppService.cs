using System.Threading.Tasks;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Accounts
{
    public class AccountAppService : IAccountAppService
    {
        private readonly LoginManager _loginManager;
        private readonly UnitOfWork _mainUnitOfWork;
        private readonly LogManager _logManager;

        public AccountAppService(UnitOfWork mainUnitOfWork, LoginManager loginManager, LogManager logManager)
        {
            _mainUnitOfWork = mainUnitOfWork;
            _loginManager = loginManager;
            _logManager = logManager;
        }

        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            var loginResult = await _loginManager.LoginAysnc(userName, password);

            if (loginResult.Result == EnumLoginResult.登录成功)
            {
                await _mainUnitOfWork.CommitAsync();
                var content = $"【{userName}】登录系统";
                await _logManager.LogInfomationAsync(EnumLogType.System, LogTitleConstString.LoginSuccess, content);
            }
            else
            {
                var content = $"【{userName}】{loginResult.Message}";
                await _logManager.LogInfomationAsync(EnumLogType.System, LogTitleConstString.LoginFail, content);
            }

            return loginResult;
        }
    }
}
