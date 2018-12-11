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
            return await query.Include(a => a.Children).OrderBy(a => a.Sort).ToListAsync();
        }

        public async Task<Department> GetAsync(string code, int? parentId)
        {
            return await _dbContext.Departments.AsNoTracking().SingleOrDefaultAsync(d => d.ParentId == parentId && d.Code.Equals(code));
        }

        public async Task<int> GetCountAsync(int parentId)
        {
            return await _dbContext.Departments.CountAsync(d => d.ParentId == parentId);
        }

        public async Task<PageResultModel<Department>> GetPagingAysnc(PageSearchCommonModel pageSearchModel)
        {
            var query = _dbContext.Departments.Include(a => a.Parent).Include(a => a.CreateUser).Include(a => a.UpdateUser).Where(a => a.ParentId == pageSearchModel.ParentId);
            if (!string.IsNullOrWhiteSpace(pageSearchModel.Keywords))
            {
                query = query.Where(q =>
                    q.Name.Contains(pageSearchModel.Keywords) || q.FullName.Contains(pageSearchModel.Keywords) || q.Code.Contains(pageSearchModel.Keywords));
            }

            var pageResultEntity = new PageResultModel<Department>
            {
                Total = await query.CountAsync(),
                Rows = await query.OrderBy(a => a.Sort).Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };

            return pageResultEntity;
        }
    }
}
