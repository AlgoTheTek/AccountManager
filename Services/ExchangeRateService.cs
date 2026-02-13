using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Models;

namespace AccountManager.Services
{
    public static class ExchangeRateService
    {
        public static Dictionary<CurrencyPair, decimal> ExtractExchangeRates(
            IEnumerable<ExchangeRateConfig> configs)
        {
            return configs.ToDictionary(
                x => new CurrencyPair(x.From, x.To),
                x => x.Rate);
        }
    }
}
