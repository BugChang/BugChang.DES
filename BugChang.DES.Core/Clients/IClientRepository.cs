using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Clients
{
    public interface IClientRepository : IBasePageSearchRepository<Client>
    {
        Task<Client> GetClient(string deviceCode);
    }
}
