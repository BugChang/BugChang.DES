using System.Threading.Tasks;
using BugChang.DES.Application.Letters.Dtos;

namespace BugChang.DES.Application.Letters
{
    public interface ILetterAppService
    {
        Task<ReceiveLetterEditDto> GetReceiveLetter(int letterId);
        Task<SendLetterEditDto> GetSendLetter(int letterId);
    }
}
