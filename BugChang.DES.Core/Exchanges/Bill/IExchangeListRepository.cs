using System;
using System.Collections.Generic;
using System.Text;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Bill
{
    public interface IExchangeListRepository : IBaseRepository<ExchangeList>
    {
    }

    public interface IExchangeListDetailRepository : IBaseRepository<ExchangeListDetail>
    {

    }
}
