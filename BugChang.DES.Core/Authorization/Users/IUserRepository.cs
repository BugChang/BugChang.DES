using System.Threading.Tasks;
using BugChang.DES.Core.IRepository;

namespace BugChang.DES.Core.Authorization.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> CheckAsync(string userName, string password);
    }
}
