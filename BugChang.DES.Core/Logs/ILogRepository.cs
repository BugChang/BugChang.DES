using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Logs
{
    public interface ILogRepository
    {
        Task AddAsync(Log log);

        Task<PageResultModel<Log>> GetSystemLogs(LogPageSerchModel pageSearchModel);

        Task<PageResultModel<Log>> GetAuditLogs(LogPageSerchModel pageSearchModel);
    }
}
