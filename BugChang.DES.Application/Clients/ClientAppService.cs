using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Clients.Dtos;
using BugChang.DES.Core.Clients;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Clients
{
    public class ClientAppService : IClientAppService
    {
        private readonly ClientManager _clientManager;
        private readonly IClientRepository _clientRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        public ClientAppService(IClientRepository clientRepository, UnitOfWork unitOfWork, ClientManager clientManager, LogManager logManager)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
            _clientManager = clientManager;
            _logManager = logManager;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(ClientEditDto editDto)
        {
            var barcodeRule = Mapper.Map<Client>(editDto);
            var result = await _clientManager.AddOrUpdateAsync(barcodeRule);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                if (editDto.Id > 0)
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.ClientEdit,
                        $"【{editDto.Name}】", JsonConvert.SerializeObject(barcodeRule), editDto.CreateBy);
                }
                else
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.ClientAdd,
                        $"【{editDto.Name}】", JsonConvert.SerializeObject(barcodeRule), editDto.CreateBy);
                }

            }
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var client = await GetForEditByIdAsync(id);
            await _clientRepository.DeleteByIdAsync(id);
            await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.ClientDelete,
                $"【{client.Name}】已删除", JsonConvert.SerializeObject(client), userId);
            var result = new ResultEntity { Success = true };
            return result;

        }

        public async Task<ClientEditDto> GetForEditByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return Mapper.Map<ClientEditDto>(client);
        }

        public async Task<PageResultModel<ClientListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            var clients = await _clientRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<ClientListDto>>(clients);

        }

        public async Task<ClientEditDto> GetClient(string deviceCode)
        {
            var client = await _clientRepository.GetClient(deviceCode);
            return Mapper.Map<ClientEditDto>(client);
        }
    }
}
