using System.Threading.Tasks;
using BugChang.DES.Core.Common;

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
            if (user.Id > 0)
            {

            }
            else
            {
                if (await ExistUserNameAsync(user.UserName))
                {

                }
            }

            return new ResultEntity();
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
