using System.Threading.Tasks;

namespace BugChang.DES.Core.Authorization.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetAsync(string userName, string password);

        Task<User> GetAsync(string userName);
    }
}
