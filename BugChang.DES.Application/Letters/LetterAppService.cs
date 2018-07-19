using System;
using System.Threading.Tasks;
using BugChang.DES.Application.Letters.Dtos;

namespace BugChang.DES.Application.Letters
{
    public class LetterAppService : ILetterAppService
    {
        public Task<ReceiveLetterEditDto> GetReceiveLetter(int letterId)
        {
            throw new NotImplementedException();
        }

        public Task<SendLetterEditDto> GetSendLetter(int letterId)
        {
            throw new NotImplementedException();
        }
    }
}
