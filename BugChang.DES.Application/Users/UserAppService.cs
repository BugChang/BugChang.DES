using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager _userManager;
        private readonly UnitOfWork _unitOfWork;

        public UserAppService(IUserRepository userRepository, UserManager userManager, UnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<UserListDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Mapper.Map<IList<UserListDto>>(users);
        }

        public async Task<ResultEntity> AddOrUpdateAsync(UserEditDto userEditDto)
        {
            var result = await _userManager.AddOrUpdateAsync(Mapper.Map<User>(userEditDto));
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id)
        {
            var result = await _userManager.DeleteByIdAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<UserEditDto> GetForEditByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return Mapper.Map<UserEditDto>(user);
        }

        public async Task<PageResultModel<UserListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var users = await _userRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<UserListDto>>(users);
        }
    }
}
