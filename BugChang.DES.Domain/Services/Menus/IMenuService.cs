using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Domain.Entities;

namespace BugChang.DES.Domain.Services.Menus
{
    public interface IMenuService
    {
        Task<IList<Menu>> GetUserMenusAsync(int userId);
    }
}
