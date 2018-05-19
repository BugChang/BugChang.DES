using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;
using BugChang.DES.Domain.IRepositories;
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
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.UserName.Equals(userName.Trim()) && u.Password.Equals(password.Trim()));
            return user;
        }
    }
}
