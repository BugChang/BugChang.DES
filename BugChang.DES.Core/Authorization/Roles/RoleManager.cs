using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Roles
{
    public class RoleManager
    {
        private readonly IRoleRepository _roleRepository;

        public RoleManager(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Role role)
        {
            //角色名称不允许重复
            var result = new ResultEntity();
            var dataBaseRole = await _roleRepository.GetByName(role.Name);
            if (dataBaseRole != null)
            {
                if (role.Id > 0)
                {
                    if (dataBaseRole.Name.Equals(role.Name) && dataBaseRole.Id == role.Id)
                    {
                        _roleRepository.Update(role);
                        result.Success = true;
                    }
                    else
                        result.Message = "角色名称已存在";
                }
                else
                    result.Message = "角色名称已存在";
            }
            else
            {
                _roleRepository.Update(role);
                result.Success = true;
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int userId)
        {
            var result = new ResultEntity();
            var role = await _roleRepository.GetByIdAsync(userId);
            role.IsDeleted = true;
            result.Success = true;
            return result;
        }

    }
}
