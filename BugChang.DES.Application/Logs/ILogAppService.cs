using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Logs.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;

namespace BugChang.DES.Application.Logs
{
    public interface ILogAppService
    {
        Task<PageResultModel<AuditLogListDto>> GetAuditLogs(LogPageSerchModel pageSearchModel);

        Task<PageResultModel<SystemLogListDto>> GetSystemLogs(LogPageSerchModel pageSearchModel);


    }
}
