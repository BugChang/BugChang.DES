using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.HardWares
{
    public interface IHardWareRepository : IBaseRepository<HardWare>
    {
        Task<IList<HardWare>> GetSettings(string deviceCode);
    }
}
