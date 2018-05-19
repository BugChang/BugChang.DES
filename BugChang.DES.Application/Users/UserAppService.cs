using BugChang.DES.Domain.IRepositories;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Application.UserApp
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;

        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddAsync(User user)
        {
            await _userRepository.AddAsync(user);
        }
    }
}
