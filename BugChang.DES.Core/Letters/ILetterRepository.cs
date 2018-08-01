using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Letters
{
    public interface ILetterRepository : IBaseRepository<Letter>
    {
        Task<PageResultModel<Letter>> GetTodayReceiveLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<Letter>> GetReceiveLetters(ReceivePageSerchModel pageSearch);
        Task<PageResultModel<Letter>> GetManagerReceiveLetters(ReceivePageSerchModel pageSearch);

        Task<Letter> GetLetter(string barcodeNo);
    }
}
