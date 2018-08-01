using BugChang.DES.Application.Clients.Dtos;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Clients
{
    public interface IClientAppService : ICurdAppService<ClientEditDto, ClientListDto>
    {
    }
}
