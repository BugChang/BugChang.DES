using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Departments
{
    public class DepartmentManager
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentManager(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Department department)
        {
            var result = new ResultEntity();

            #region 检查机构代码

            if (!department.CheckCode())
            {
                result.Message = "机构代码输入内容错误";
                return result;
            }
            if (await ExistCodeAsync(department))
            {
                result.Message = "机构代码重复";
                return result;
            }

            #endregion

            //设置机构全称
            if (department.ParentId != null)
            {
                var parentDepartment = await _departmentRepository.GetAsync(department.ParentId.Value);
                department.SetFullName(parentDepartment);
            }


            if (department.Id > 0)
            {
                _departmentRepository.Update(department);
            }
            else
            {
                await _departmentRepository.AddAsync(department);
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// 检查代码是否重复
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task<bool> ExistCodeAsync(Department department)
        {
            var dataBaseDepartment = await _departmentRepository.GetAsync(department.Code, department.ParentId);
            if (dataBaseDepartment == null)
            {
                return false;
            }
            return department.Id != dataBaseDepartment.Id;
        }

        public async Task<Department> GetAsync(int id)
        {
            return await _departmentRepository.GetAsync(id);
        }

        public async Task<IList<Department>> GetAllAsync(int? parentId)
        {
            return await _departmentRepository.GetAllAsync(parentId);
        }

        public async Task<IList<Department>> GetAllAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<PageResultEntity<Department>> GetPagingAysnc(int? parentId, int take, int skip)
        {
            return await _departmentRepository.GetPagingAysnc(parentId, take, skip);
        }
    }
}
