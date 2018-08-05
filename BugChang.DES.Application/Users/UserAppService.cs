using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager _userManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        public UserAppService(IUserRepository userRepository, UserManager userManager, UnitOfWork unitOfWork, LogManager logManager, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _roleRepository = roleRepository;
        }

        public async Task<IList<UserListDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Mapper.Map<IList<UserListDto>>(users);
        }

        public async Task<IList<RoleListDto>> GetUserRoles(int userId)
        {
            return Mapper.Map<IList<RoleListDto>>(await _userManager.GetUserRoles(userId));
        }

        public async Task<ResultEntity> AddUserRole(int userId, int roleId, int operatorId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            var result = await _userManager.AddUserRole(userRole);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                //写日志
                var user = await _userRepository.GetByIdAsync(userId);
                var role = await _roleRepository.GetByIdAsync(roleId);
                var logContent = $"用户【{user.DisplayName}】添加了角色【{role.Name}】";
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.UserRoleAdd, logContent, null, operatorId);
            }

            return result;
        }

        public async Task<ResultEntity> DeleteUserRole(int userId, int roleId, int operatorId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            var result = await _userManager.DeleteUserRole(userRole);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                //写日志
                var user = await _userRepository.GetByIdAsync(userId);
                var role = await _roleRepository.GetByIdAsync(roleId);
                var logContent = $"用户【{user.DisplayName}】删除了角色【{role.Name}】";
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.UserRoleDelete, logContent, null, operatorId);
            }

            return result;
        }

        public async Task<ResultEntity> ChangeUserEnabled(int userId, int operatorId)
        {
            var result = new ResultEntity();
            var user = await _userRepository.GetByIdAsync(userId);
            user.Enabled = !user.Enabled;
            result.Success = true;
            await _unitOfWork.CommitAsync();
            await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.UserEnabled, $"用户【{user.DisplayName}】已被【{(user.Enabled ? "启用" : "禁用")}】", null, operatorId);
            return result;

        }

        public async Task<ResultEntity> AddOrUpdateAsync(UserEditDto userEditDto)
        {
            var user = Mapper.Map<User>(userEditDto);
            var result = await _userManager.AddOrUpdateAsync(user);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                if (userEditDto.Id > 0)
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.UserEdit,
                        $"【{userEditDto.DisplayName}】", JsonConvert.SerializeObject(user), userEditDto.UpdateBy);
                }
                else
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.UserAdd,
                        $"【{userEditDto.DisplayName}】", JsonConvert.SerializeObject(user), userEditDto.CreateBy);
                }
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int operatorId)
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

        public async Task<PageResultModel<UserListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            var users = await _userRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<UserListDto>>(users);
        }
    }
}
