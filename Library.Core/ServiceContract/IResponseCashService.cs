using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface IResponseCashService
    {
        Task CashResponseAsync(string CashKey, object Response, TimeSpan TimeToLive);
        Task<string?> GetCashResponseAsync(string CashKey);
    }
}
