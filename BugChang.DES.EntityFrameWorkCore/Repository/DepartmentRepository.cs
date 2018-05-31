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

        public async Task<PageResultEntity<Department>> GetPagingAysnc(int? parentId, int take, int skip, string keywords)
        {
            var query = from department in _dbContext.Departments
                        where department.ParentId == parentId
                        select department;
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(q =>
                    q.Name.Contains(keywords) || q.FullName.Contains(keywords) || q.Code.Contains(keywords));
            }
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

        public async Task<int> GetCountAsync(int parentId)
        {
            return await _dbContext.Departments.CountAsync(d => d.ParentId == parentId);
        }

        public async Task<Department> GetViewAsync(int id)
        {
            return await _dbContext.Departments.Include(a => a.Parent).Include(a => a.CreateUser).Include(a => a.UpdateUser)
                .SingleOrDefaultAsync(a => a.Id == id);
        }
    }
}
