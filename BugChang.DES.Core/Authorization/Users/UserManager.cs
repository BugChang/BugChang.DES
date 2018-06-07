using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using NETCore.Encrypt.Extensions;

namespace BugChang.DES.Core.Authorization.Users
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(User user)
        {
            var result = new ResultEntity();
            if (user.Id > 0)
            {

            }
            else
            {
                if (await ExistUserNameAsync(user.UserName))
                {
                    result.Message = "已存在的用户名";
                }
                else
                {
                    user.Password = User.DefaultPassword.MD5();
                    await _userRepository.AddAsync(user);
                    result.Success = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> ExistUserNameAsync(string userName)
        {
            var dataBaseUser = await _userRepository.GetAsync(userName);
            if (dataBaseUser == null)
            {
                return false;
            }

            return true;
        }
    }
}
