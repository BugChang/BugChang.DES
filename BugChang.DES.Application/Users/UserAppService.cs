using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Authorization.Users;

namespace BugChang.DES.Application.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;

        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IList<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Mapper.Map<IList<UserDto>>(users);
        }
    }
}
