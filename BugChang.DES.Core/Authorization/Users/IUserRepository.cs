using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Users
{
    public interface IUserRepository : IBasePageSearchRepository<User>
    {
        Task<User> GetAsync(string userName, string password);

        Task<User> GetAsync(string userName);
        
        Task<int> GetCountAsync(int departmentId);
    }
}
