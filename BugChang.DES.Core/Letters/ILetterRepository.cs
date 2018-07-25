using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Letters
{
    public interface ILetterRepository : IBasePageSearchRepository<Letter>
    {
        Task<PageResultModel<Letter>> GetTodayReceiveLetters(PageSearchModel pageSearchModel);
    }
}
