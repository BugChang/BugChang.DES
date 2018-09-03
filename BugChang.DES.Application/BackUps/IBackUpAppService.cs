using System.Threading.Tasks;
using BugChang.DES.Application.BackUps.Dtos;
using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.BackUps
{
    public interface IBackUpAppService : ICurdAppService<BackUpEditDto, BackUpListDto>
    {
        Task<ResultEntity> BackUpNow(int type, string operatorName, string remark);
    }
}
