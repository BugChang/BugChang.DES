using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        private readonly DesDbContext _dbContext;
        public DepartmentRepository(DesDbContext dbContext) : base(dbContext)
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

        public async Task<Department> GetAsync(string code, int? parentId)
        {
            return await _dbContext.Departments.AsNoTracking().SingleOrDefaultAsync(d => d.ParentId == parentId && d.Code.Equals(code));
        }

        public async Task<int> GetCountAsync(int parentId)
        {
            return await _dbContext.Departments.CountAsync(d => d.ParentId == parentId);
        }

        public async Task<PageResultModel<Department>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            var query = from department in _dbContext.Departments
                        where department.ParentId == pageSearchModel.ParentId
                        select department;
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(q =>
                    q.Name.Contains(pageSearchModel.Keywords) || q.FullName.Contains(pageSearchModel.Keywords) || q.Code.Contains(pageSearchModel.Keywords));
            }

            query = query.Include(a => a.Parent).Include(a => a.CreateUser).Include(a => a.UpdateUser);
            var pageResultEntity = new PageResultModel<Department>
            {
                Total = await query.CountAsync(),
                Rows = await query.Take(pageSearchModel.Take).Skip(pageSearchModel.Skip).ToListAsync()
            };

            return pageResultEntity;
        }
    }
}
