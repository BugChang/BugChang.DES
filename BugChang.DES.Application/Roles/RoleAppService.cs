using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Commons;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Roles
{
    public class RoleAppService : IRoleAppService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly RoleManager _roleManager;
        private readonly UnitOfWork _unitOfWork;
        public RoleAppService(IRoleRepository roleRepository, RoleManager roleManager, UnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(RoleEditDto editDto)
        {
            var result = await _roleManager.AddOrUpdateAsync(Mapper.Map<Role>(editDto));
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = await _roleManager.DeleteByIdAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<RoleEditDto> GetForEditByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return Mapper.Map<RoleEditDto>(role);
        }

        public async Task<PageResultModel<RoleListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            var roles = await _roleRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<RoleListDto>>(roles);
        }

        public async Task<ResultEntity> EditRoleMenu(int roleId, IList<int> lstMenuId)
        {
            var result = await _roleManager.EditRoleMenu(roleId, lstMenuId);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public IList<string> GetRoleOperationCodes(string module, List<int> lstRoleId)
        {
            return _roleManager.GetRoleOperationCodes(module, lstRoleId);
        }

        public async Task<ResultEntity> AddRoleOperation(int roleId, string operationCode)
        {
            var result = await _roleManager.AddRoleOperation(roleId, operationCode);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<ResultEntity> DeleteRoleOperation(int roleId, string operationCode)
        {
            var result = _roleManager.DeleteRoleOperation(roleId, operationCode);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<IList<RoleListDto>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Mapper.Map<IList<RoleListDto>>(roles);
        }
    }
}
