using System;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Authorization.Roles;
using BugChang.DES.Core.Authorization.Users;
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

        public async Task<ResultEntity> DeleteByIdAsync(int id)
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

        public async Task<PageResultModel<RoleListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var roles = await _roleRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<RoleListDto>>(roles);
        }
    }
}
