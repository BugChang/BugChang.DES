using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Common;
using BugChang.DES.Core.Departments;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        private readonly MainDbContext _dbContext;
        public DepartmentRepository(MainDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Department>> GetAllAsync(int? parentId)
        {
            var query = from department in _dbContext.Departments
                        where department.ParentId == parentId
                        select department;
            return await query.Include(a => a.Children).ToListAsync();
        }

        public async Task<PageResultEntity<Department>> GetPagingAysnc(int? parentId, int take, int skip)
        {
            var query = from department in _dbContext.Departments
                        where department.ParentId == parentId
                        select department;

            var pageResultEntity = new PageResultEntity<Department>
            {
                Total = await query.CountAsync(),
                Rows = await query.Take(take).Skip(skip).ToListAsync()
            };

            return pageResultEntity;
        }

        public async Task<Department> GetAsync(string code, int? parentId)
        {
            return await _dbContext.Departments.AsNoTracking().SingleOrDefaultAsync(d => d.ParentId == parentId && d.Code.Equals(code));
        }
    }
}
