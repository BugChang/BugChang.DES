using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Users;

namespace BugChang.DES.Application.UserApp
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;

        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CheckAsync(string userName, string password)
        {
            return await _userRepository.CheckAsync(userName, password);
        }

        public async Task AddAsync(User user)
        {
            await _userRepository.AddAsync(user);   
        }
    }
}
