using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NETCore.Encrypt.Extensions;

namespace BugChang.DES.Core.Authorization.Users
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserManager(IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(User user)
        {
            var result = new ResultEntity();
            if (user.Id > 0)
            {
                var dataBaseUser = await _userRepository.GetAsync(user.UserName);
                if (dataBaseUser != null && dataBaseUser.UserName != user.UserName)
                {
                    result.Message = "已存在的用户名";
                }
                else
                {
                    _userRepository.Update(user);
                    result.Success = true;
                }
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

        public async Task<ResultEntity> DeleteByIdAsync(int userId)
        {
            var result = new ResultEntity();
            var user = await _userRepository.GetByIdAsync(userId);
            user.IsDeleted = true;
            result.Success = true;
            return result;
        }

        public async Task<IList<Role>> GetUserRoles(int userId)
        {
            return await _userRoleRepository.GetQueryable().Where(a => a.User.Id == userId).Select(a => a.Role).ToListAsync();
        }

        public async Task<ResultEntity> AddUserRole(UserRole userRole)
        {
            var resultEntity = new ResultEntity();
            if (await ExistUserRole(userRole))
            {
                resultEntity.Message = "已经存在的角色，请勿重复添加！";
            }
            else
            {
                await _userRoleRepository.AddAsync(userRole);
                resultEntity.Success = true;
            }

            return resultEntity;
        }

        public async Task<ResultEntity> DeleteUserRole(UserRole userRole)
        {
            var resultEntity = new ResultEntity();
            var dataBaseUserRole = await _userRoleRepository.GetQueryable().FirstOrDefaultAsync(a => a.UserId == userRole.UserId && a.RoleId == userRole.RoleId);
            if (dataBaseUserRole == null)
            {
                resultEntity.Message = "该用户无此角色！";
            }
            else
            {
                dataBaseUserRole.IsDeleted = true;
                resultEntity.Success = true;
            }

            return resultEntity;
        }

        public async Task<bool> ExistUserRole(UserRole userRole)
        {
            var existUserRole = await _userRoleRepository.GetQueryable()
                                    .CountAsync(a => a.UserId == userRole.UserId && a.RoleId == userRole.RoleId) > 0;
            return existUserRole;
        }

    }
}
