using System.Threading.Tasks;
using BugChang.DES.Application.Cards.Dtos;
using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Cards
{
    public interface ICardAppService : ICurdAppService<CardEditDto, CardListDto>
    {
        Task<ResultEntity> ChangeEnabled(int cardId, int operatorId);

        Task<CardEditDto> GetCardByNo(string cardNo);
    }
}
