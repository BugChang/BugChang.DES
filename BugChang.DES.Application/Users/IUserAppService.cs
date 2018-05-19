using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Application.UserApp
{
    public interface IUserAppService
    {
        Task AddAsync(User user);
    }
}
