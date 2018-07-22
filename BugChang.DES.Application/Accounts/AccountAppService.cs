using System.Threading.Tasks;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Accounts
{
    public class AccountAppService : IAccountAppService
    {
        private readonly LoginManager _loginManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly UserManager _userManager;

        public AccountAppService(UnitOfWork mainUnitOfWork, LoginManager loginManager, LogManager logManager, UserManager userManager)
        {
            _unitOfWork = mainUnitOfWork;
            _loginManager = loginManager;
            _logManager = logManager;
            _userManager = userManager;
        }

        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            var loginResult = await _loginManager.LoginAysnc(userName, password);

            if (loginResult.Result == EnumLoginResult.登录成功 || loginResult.Result == EnumLoginResult.强制修改密码)
            {
                await _unitOfWork.CommitAsync();
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

        public async Task<ResultEntity> ChangePassword(int userId, string password, string oldPassword)
        {
            var result = await _userManager.ChangePassword(userId, password, oldPassword);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.System, "密码修改成功", result.Message);
            }

            return result;
        }
    }
}
