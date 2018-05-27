using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Core.Common;
using BugChang.DES.Core.Departments;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Application.Departments
{
    public class DepartmentAppService : IDepartmentAppService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly UnitOfWork<MainDbContext> _unitOfWork;

        public DepartmentAppService(IDepartmentRepository departmentRepository, UnitOfWork<MainDbContext> unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddOrUpdateAsync(DepartmentDto department)
        {
            if (department.Id > 0)
            {
                await _departmentRepository.UpdateAsync(Mapper.Map<Department>(department));
            }
            else
            {
                await _departmentRepository.AddAsync(Mapper.Map<Department>(department));
            }

            await _unitOfWork.CommitAsync();
        }


        public async Task<IList<DepartmentDto>> GetAllAsync(int? parentId)
        {
            var departments = await _departmentRepository.GetAllAsync(parentId);
            return Mapper.Map<IList<DepartmentDto>>(departments);
        }



        public async Task<PageResultEntity<DepartmentDto>> GetPagingAysnc(int? parentId, int limit, int offset)
        {
            var pageResult = await _departmentRepository.GetPagingAysnc(parentId, limit, offset);
            return Mapper.Map<PageResultEntity<DepartmentDto>>(pageResult);
        }
    }
}
