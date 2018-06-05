using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Core.Common;
using BugChang.DES.Core.Departments;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Departments
{
    public class
        DepartmentAppService : IDepartmentAppService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly DepartmentManager _departmentManager;
        public DepartmentAppService(UnitOfWork unitOfWork, DepartmentManager departmentManager)
        {
            _unitOfWork = unitOfWork;
            _departmentManager = departmentManager;
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

        public async Task<ResultEntity> DeleteAsync(int id)
        {
            var result = await _departmentManager.DeleteAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }


        /// <summary>
        /// 根据parentId获取全部机构
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<IList<DepartmentDto>> GetAllAsync(int? parentId)
        {
            var departments = await _departmentManager.GetAllAsync(parentId);
            return Mapper.Map<IList<DepartmentDto>>(departments);
        }

        /// <summary>
        /// 获取全部机构
        /// </summary>
        /// <returns></returns>
        public async Task<IList<DepartmentDto>> GetAllAsync()
        {
            var departments = await _departmentManager.GetAllAsync();
            return Mapper.Map<IList<DepartmentDto>>(departments);
        }

        public async Task<DepartmentEditDto> GetAsync(int id)
        {
            return Mapper.Map<DepartmentEditDto>(await _departmentManager.GetAsync(id));
        }


        /// <summary>
        /// 分页获取机构数据
        /// </summary>
        /// <param name="parentId">父Id</param>
        /// <param name="take">查询条数</param>
        /// <param name="skip">跳过条数</param>
        /// <param name="keywords">关键字</param>
        /// <returns></returns>
        public async Task<PageResultEntity<DepartmentDto>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            var pageResult = await _departmentManager.GetPagingAysnc(parentId, take, skip, keywords);
            return Mapper.Map<PageResultEntity<DepartmentDto>>(pageResult);
        }
    }
}
