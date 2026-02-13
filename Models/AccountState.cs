using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Models
{
    public record AccountState(DateTime Date, decimal BalanceEur);
    public record ExchangeRate(string From, string To, decimal Rate);
}
