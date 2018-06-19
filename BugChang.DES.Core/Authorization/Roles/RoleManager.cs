using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Roles
{
    public class RoleManager
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleMenuRepository _roleMenuRepository;
        private readonly IRoleOperationRepository _roleOperationRepository;

        public RoleManager(IRoleRepository roleRepository, IRoleMenuRepository roleMenuRepository, IRoleOperationRepository roleOperationRepository)
        {
            _roleRepository = roleRepository;
            _roleMenuRepository = roleMenuRepository;
            _roleOperationRepository = roleOperationRepository;
        }

        /// <summary>
        /// 新增或修改角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 通过Id删除角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultEntity> DeleteByIdAsync(int userId)
        {
            var result = new ResultEntity();
            var role = await _roleRepository.GetByIdAsync(userId);
            role.IsDeleted = true;
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 修改角色和菜单的关联关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="lstMenuId"></param>
        /// <returns></returns>
        public async Task<ResultEntity> EditRoleMenu(int roleId, IList<int> lstMenuId)
        {
            var resultEntity = new ResultEntity();

            //生成本次批次号
            var roleMenu = _roleMenuRepository.GetQueryable().FirstOrDefault(a => a.RoleId == roleId);
            var batchNo = roleMenu?.BatchNo + 1 ?? 1;

            //删除现有数据
            DeleteRoleMenus(roleId);
            //添加新增数据
            foreach (var menuId in lstMenuId)
            {
                var newRoleMenu = new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId,
                    BatchNo = batchNo
                };
                await _roleMenuRepository.AddAsync(newRoleMenu);
            }


            resultEntity.Success = true;
            return resultEntity;
        }

        /// <summary>
        /// 删除角色和菜单关联
        /// </summary>
        /// <param name="roleId"></param>
        private void DeleteRoleMenus(int roleId)
        {
            var roleMenus = _roleMenuRepository.GetQueryable().Where(a => a.RoleId == roleId);
            foreach (var roleMenu in roleMenus)
            {
                roleMenu.IsDeleted = true;
            }
        }

        /// <summary>
        /// 获取操作代码列表
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="lstRoleId">角色ID</param>
        /// <returns></returns>
        public IList<string> GetRoleOperationCodes(string module, List<int> lstRoleId)
        {
            var roleOperationCodes = _roleOperationRepository.GetQueryable().Where(a => lstRoleId.Any(r => r == a.RoleId) && a.OperationCode.Contains(module))
                .Select(a => a.OperationCode).ToList();
            return roleOperationCodes;
        }

        /// <summary>
        /// 添加角色和操作关联
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="operationCode">操作代码</param>
        /// <returns></returns>
        public async Task<ResultEntity> AddRoleOperation(int roleId, string operationCode)
        {
            var result = new ResultEntity();
            var roleOperation = _roleOperationRepository.GetQueryable()
                .FirstOrDefault(a => a.RoleId == roleId && a.OperationCode == operationCode);
            if (roleOperation != null)
            {
                result.Message = "已经存在的操作，请勿重复添加！";
            }
            else
            {
                var newRoleOperation = new RoleOperation()
                {
                    RoleId = roleId,
                    OperationCode = operationCode
                };
                await _roleOperationRepository.AddAsync(newRoleOperation);
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        ///删除角色和操作关联
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="operationCode">操作代码</param>
        /// <returns></returns>
        public ResultEntity DeleteRoleOperation(int roleId, string operationCode)
        {
            var result = new ResultEntity();
            var roleOperation = _roleOperationRepository.GetQueryable()
                .FirstOrDefault(a => a.RoleId == roleId && a.OperationCode == operationCode);
            if (roleOperation == null)
            {
                result.Message = "删除失败，不存在的操作条目！";
            }
            else
            {
                roleOperation.IsDeleted = true;
                result.Success = true;
            }

            return result;
        }
    }
}
