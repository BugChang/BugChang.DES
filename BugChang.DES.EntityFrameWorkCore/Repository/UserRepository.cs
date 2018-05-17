using System.Threading.Tasks;
using BugChang.DES.Core.Authorization.Users;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.EntityFrameWorkCore.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public async Task<bool> CheckAsync(string userName, string password)
        {
            using (var dbContext = new BasicDdContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.UserName.Equals(userName) && u.Password.Equals(password));
                return user != null;
            }
        }
    }
}
