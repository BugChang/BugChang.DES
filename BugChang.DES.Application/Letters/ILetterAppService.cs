using System.Threading.Tasks;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Letters;

namespace BugChang.DES.Application.Letters
{
    public interface ILetterAppService
    {
        Task<ReceiveLetterEditDto> GetReceiveLetter(int letterId);
        Task<LetterSendEditDto> GetSendLetter(int letterId);

        Task<ResultEntity> AddReceiveLetter(ReceiveLetterEditDto receiveLetter);

        Task<PageResultModel<LetterReceiveListDto>> GetTodayReceiveLetters(PageSearchCommonModel pageSearchModel);

        Task<LetterReceiveBarcodeDto> GetReceiveBarcode(int letterId);

        Task<PageResultModel<LetterReceiveListDto>> GetReceiveLetters(LetterPageSerchModel pageSearchModel);

        Task<ResultEntity> AddSendLetter(LetterSendEditDto sendLetter);

        Task<PageResultModel<LetterSendListDto>> GetTodaySendLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<LetterSendListDto>> GetSendLetters(LetterPageSerchModel pageSearchModel);
    }
}
