using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Users;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly MainDbContext _dbContext;
        public UserRepository(MainDbContext dbContext) : base(dbContext)
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
            var user = await _dbContext.Users.SingleOrDefaultAsync(u =>
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
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.UserName.Equals(userName.Trim()));
            return user;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="limit">查询数量</param>
        /// <param name="offset">跳过数量</param>
        /// <param name="keywords">关键字</param>
        /// <returns></returns>
        public async Task<IList<User>> GetAllAsync(int limit, int offset, string keywords)
        {
            var query = _dbContext.Users.Where(a => a.Enabled);
            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(a => a.DisplayName.Contains(keywords) || a.UserName.Contains(keywords));
            }

            var users = await query.ToListAsync();
            return users;
        }
    }
}
