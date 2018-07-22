using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Authentication.Card
{
    public class CardManager
    {
        private readonly ICardRepository _cardRepository;

        public CardManager(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Card card)
        {
            var result = new ResultEntity();
            var exist = await _cardRepository.GetQueryable().Where(a => a.Value == card.Value && a.Id != card.Id)
                .CountAsync() > 0;
            if (exist)
            {
                result.Message = "整卡卡号重复";
            }
            else
            {
                if (card.Id > 0)
                {
                    _cardRepository.Update(card);
                }
                else
                {
                    await _cardRepository.AddAsync(card);
                }

                result.Success = true;
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int cardId)
        {
            var result = new ResultEntity();
            var card = await _cardRepository.GetQueryable().SingleOrDefaultAsync(a => a.Id == cardId);
            card.IsDeleted = true;
            result.Success = true;
            result.Message = $"【证卡编号：{card.Number}】已被删除";
            result.Data = card;
            return result;
        }

        public async Task<ResultEntity> ChangeEnabled(int cardId)
        {
            var result = new ResultEntity();
            var card = await _cardRepository.GetQueryable().SingleOrDefaultAsync(a => a.Id == cardId);
            card.Enabled = !card.Enabled;
            result.Success = true;
            result.Message = $"【证卡编号：{card.Number}】已被" + (card.Enabled ? "启用" : "禁用");
            result.Data = card;
            return result;
        }
    }
}
