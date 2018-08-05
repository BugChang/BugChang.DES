using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Letters
{
    public interface ILetterRepository : IBaseRepository<Letter>
    {
        Task<PageResultModel<Letter>> GetTodayReceiveLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<Letter>> GetTodaySendLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<Letter>> GetReceiveLetters(LetterPageSerchModel pageSearch);

        Task<PageResultModel<Letter>> GetManagerReceiveLetters(LetterPageSerchModel pageSearch);

        Task<Letter> GetLetter(string barcodeNo);

        Task<PageResultModel<Letter>> GetSendLetters(LetterPageSerchModel pageSearch);
    }
}
