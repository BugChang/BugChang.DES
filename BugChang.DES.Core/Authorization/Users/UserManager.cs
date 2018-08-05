using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NETCore.Encrypt.Extensions;

namespace BugChang.DES.Core.Authorization.Users
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IOptions<AccountSettings> _accountSettings;

        public UserManager(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IOptions<AccountSettings> accountSettings)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _accountSettings = accountSettings;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(User user)
        {
            var result = new ResultEntity();
            var existUserName = await _userRepository
                .GetQueryable().AnyAsync(a => a.UserName == user.UserName && a.Id != user.Id);
            if (existUserName)
            {
                result.Message = "已存在的用户名";
            }
            else
            {
                if (user.Id > 0)
                {
                    _userRepository.Update(user);
                }
                else
                {
                    user.Password = User.DefaultPassword.MD5();
                    await _userRepository.AddAsync(user);
                }
                result.Success = true;
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

        public async Task<ResultEntity> ChangePassword(int userId, string password, string oldPassword)
        {
            var result = new ResultEntity();
            if (password.Length < _accountSettings.Value.PasswordMinLength)
            {
                result.Message = $"密码长度至少{_accountSettings.Value.PasswordMinLength}位";
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user.Password == HashHelper.Md5(oldPassword))
                {
                    if (user.Password == HashHelper.Md5(password))
                    {
                        result.Message = "不能使用原密码作为新密码";
                    }
                    else
                    {
                        user.Password = HashHelper.Md5(password);
                        result.Message = $"用户【{user.DisplayName}】成功修改了登录密码";
                        result.Success = true;
                    }
                }
                else
                {
                    result.Message = "原密码输入错误";
                }
            }
            return result;
        }
    }
}
