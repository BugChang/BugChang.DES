using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Common;

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

        public Task<ResultEntity> AddOrUpdateAsync(UserEditDto dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResultEntity> DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserEditDto> GetAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
