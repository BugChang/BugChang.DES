using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Channel;

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

        Task<PageResultModel<Letter>> GetBackLettersForSearch(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<Letter>> GetBackLettersForManagerSearch(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<Letter>> GetCancelLettersForSearch(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<Letter>> GetNoSortingLetters(EnumChannel channel);
    }

    public interface IBackLetterRepository : IBaseRepository<BackLetter>
    {
        Task<PageResultModel<Letter>> GetBackLetters(PageSearchCommonModel pageSerch);
    }

    public interface ICancelLetterRepository : IBaseRepository<CancelLetter>
    {
        Task<PageResultModel<Letter>> GetCancelLetters(PageSearchCommonModel pageSerch);

    }
}
