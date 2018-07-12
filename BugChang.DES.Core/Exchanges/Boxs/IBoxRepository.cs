using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public interface IBoxRepository : IBasePageSearchRepository<Box>
    {
    }

    public interface IBoxObjectRepository : IBaseRepository<BoxObject>
    {
        Task<IList<Box>> GetBoxsByObjectId(int objectId);
    }
}
