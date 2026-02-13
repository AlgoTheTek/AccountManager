using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Models;

namespace AccountManager.Interfaces
{
    public interface IAccountService
    {
        AccountState GetBalanceAtDate(DateTime date);
        IEnumerable<Transaction> GetTransactions(DateTime? fromDate = null, DateTime? toDate = null);
        IEnumerable<Transaction> GetAllTransactions();
        decimal GetExchangeRate(string fromCurrency, string toCurrency);
    }
}
