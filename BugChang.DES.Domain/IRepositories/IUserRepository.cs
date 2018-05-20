using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Domain.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetAsync(string userName, string password);

        Task<User> GetAsync(string userName);
    }
}
