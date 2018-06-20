using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Departments
{
    public class DepartmentAppService : IDepartmentAppService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly DepartmentManager _departmentManager;
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentAppService(UnitOfWork unitOfWork, DepartmentManager departmentManager, IDepartmentRepository departmentRepository)
        {
            _unitOfWork = unitOfWork;
            _departmentManager = departmentManager;
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// 新增或修改机构
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task<ResultEntity> AddOrUpdateAsync(DepartmentEditDto department)
        {
            var model = Mapper.Map<Department>(department);
            var result = await _departmentManager.AddOrUpdateAsync(model);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = await _departmentManager.DeleteAsync(id, userId);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<DepartmentEditDto> GetForEditByIdAsync(int id)
        {
            return Mapper.Map<DepartmentEditDto>(await _departmentManager.GetAsync(id));
        }



        /// <summary>
        /// 根据parentId获取全部机构
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<IList<DepartmentListDto>> GetAllAsync(int? parentId)
        {
            var departments = await _departmentManager.GetAllAsync(parentId);
            return Mapper.Map<IList<DepartmentListDto>>(departments);
        }

        /// <summary>
        /// 获取全部机构
        /// </summary>
        /// <returns></returns>
        public async Task<IList<DepartmentListDto>> GetAllAsync()
        {
            var departments = await _departmentManager.GetAllAsync();
            return Mapper.Map<IList<DepartmentListDto>>(departments);
        }

        /// <summary>
        /// 分页获取机构列表
        /// </summary>
        /// <param name="pageSearchDto"></param>
        /// <returns></returns>
        public async Task<PageResultModel<DepartmentListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var pageResult = await _departmentRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<DepartmentListDto>>(pageResult);
        }
    }
}
