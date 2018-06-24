using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DesDbContext _dbContext;
        public UserRepository(DesDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取一个用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>User Or Null</returns>
        public async Task<User> GetAsync(string userName, string password)
        {
            var user = await _dbContext.Users.Include(a => a.UserRoles).ThenInclude(a => a.Role).SingleOrDefaultAsync(u =>
                    u.UserName.Equals(userName.Trim()) && u.Password.Equals(password.Trim()));
            return user;
        }

        /// <summary>
        /// 获取一个用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public async Task<User> GetAsync(string userName)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u =>
                u.UserName.Equals(userName.Trim()));
            return user;
        }


        /// <summary>
        /// 获取指定机构下的用户数量
        /// </summary>
        /// <param name="departmentId">机构标识</param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(int departmentId)
        {
            return await _dbContext.Users.CountAsync(u => u.DepartmentId == departmentId);
        }

        /// <summary>
        /// 分页获取用户列表
        /// </summary>
        /// <param name="pageSearchModel"></param>
        /// <returns></returns>
        public async Task<PageResultModel<User>> GetPagingAysnc(PageSearchModel pageSearchModel)
        {
            var query = _dbContext.Users.Include(a => a.CreateUser).Include(a => a.UpdateUser)
                .Include(a => a.Department).Where(a => true);
            if (!string.IsNullOrEmpty(pageSearchModel.Keywords))
            {
                query = query.Where(a => a.DisplayName.Contains(pageSearchModel.Keywords) || a.UserName.Contains(pageSearchModel.Keywords));
            }
            var pageResult = new PageResultModel<User>
            {
                Total = await query.CountAsync(),
                Rows = await query.Skip(pageSearchModel.Skip).Take(pageSearchModel.Take).ToListAsync()
            };
            return pageResult;
        }
    }
}
