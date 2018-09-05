using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Cards.Dtos;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Cards
{
    public class CardAppService : ICardAppService
    {
        private readonly CardManager _cardManager;
        private readonly LogManager _logManager;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlaceWardenRepository _placeWardenRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly UnitOfWork _unitOfWork;
        public CardAppService(CardManager cardManager, LogManager logManager, ICardRepository cardRepository, UnitOfWork unitOfWork, IPlaceWardenRepository placeWardenRepository, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _cardManager = cardManager;
            _logManager = logManager;
            _cardRepository = cardRepository;
            _unitOfWork = unitOfWork;
            _placeWardenRepository = placeWardenRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(CardEditDto editDto)
        {
            var card = Mapper.Map<Card>(editDto);
            var result = await _cardManager.AddOrUpdateAsync(card);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                if (editDto.Id > 0)
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.CardEdit,
                        $"【{editDto.Number}】", JsonConvert.SerializeObject(card), editDto.CreateBy);
                }
                else
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.CardAdd,
                        $"【{editDto.Number}】", JsonConvert.SerializeObject(card), editDto.CreateBy);
                }
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var reult = await _cardManager.DeleteByIdAsync(id);
            if (reult.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.CardDelete,
                  reult.Message, JsonConvert.SerializeObject(reult.Data), userId);
            }

            return reult;
        }

        public async Task<CardEditDto> GetForEditByIdAsync(int id)
        {
            var card = await _cardRepository.GetByIdAsync(id);
            return Mapper.Map<CardEditDto>(card);
        }

        public async Task<PageResultModel<CardListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            var user = await _userRepository.GetByIdAsync(pageSearchDto.UserId);

            if (!"sysadmin,secadmin,audadmin".Contains(user.UserName))
            {
                var placeWarden = await _placeWardenRepository.GetQueryable().Include(a => a.Place).Where(a => a.UserId == pageSearchDto.UserId)
                    .FirstOrDefaultAsync();
                if (placeWarden != null)
                {
                    pageSearchDto.DepartmentId = placeWarden.Place.DepartmentId;
                }
                else
                {
                    return new PageResultModel<CardListDto>();
                }
            }
            var cards = await _cardRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<CardListDto>>(cards);
        }

        public async Task<ResultEntity> ChangeEnabled(int cardId, int operatorId)
        {
            var reult = await _cardManager.ChangeEnabled(cardId);
            if (reult.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.CardChangeEnabled,
                    reult.Message, JsonConvert.SerializeObject(reult.Data), operatorId);
            }
            return reult;
        }

        public async Task<CardEditDto> GetCardByNo(string cardNo)
        {
            var card = await _cardRepository.GetQueryable().FirstOrDefaultAsync(a => a.Value == cardNo);
            return Mapper.Map<CardEditDto>(card);
        }
    }
}
