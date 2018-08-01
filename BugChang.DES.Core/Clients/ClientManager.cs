using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Clients
{
    public class ClientManager
    {
        private readonly IClientRepository _clientRepository;

        public ClientManager(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Client client)
        {
            var result = new ResultEntity();
            var exist = await _clientRepository.GetQueryable().Where(a => a.Name == client.DeviceCode && a.Id != client.Id).CountAsync() > 0;
            if (exist)
            {
                result.Message = "设备码已存在";
            }
            else
            {
                if (client.Id > 0)
                {
                    _clientRepository.Update(client);
                }
                else
                {
                    await _clientRepository.AddAsync(client);
                }

                result.Success = true;
            }

            return result;
        }
    }
}
