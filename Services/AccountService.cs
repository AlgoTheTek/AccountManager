using AccountManager.Interfaces;
using AccountManager.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Services
{
    public class AccountService : IAccountService
    {
        private readonly ITransactionRepository _repository;
        private readonly IConfiguration _configuration;

        public AccountService(ITransactionRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public AccountState GetBalanceAtDate(DateTime date)
        {
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);
            var tx = _repository.FindByDateRange(DateTime.MinValue.Date, endOfDay.Date);
            var balance = tx.Sum(t => t.AmountEur);
            return new AccountState(date, balance);
        }
        public IEnumerable<Transaction> GetTransactions(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var start = fromDate ?? DateTime.MinValue;
            var end = toDate ?? DateTime.MaxValue;
            return _repository.FindByDateRange(start, end);
        }
        public IEnumerable<Transaction> GetAllTransactions() => _repository.GetAll();
        public decimal GetExchangeRate( string fromCurrency, string toCurrency)
        {
            var rates = _configuration
                .GetSection("ExchangeRates")
                .Get<List<ExchangeRateConfig>>();

            var rate = rates?.FirstOrDefault(x =>
                x.From.Equals(fromCurrency, StringComparison.OrdinalIgnoreCase) &&
                x.To.Equals(toCurrency, StringComparison.OrdinalIgnoreCase));

            return rate?.Rate ?? 1m;
        }
        public decimal GetBalanceAtDate(DateOnly targetDate)
        {
            var transactions = GetAllTransactions()
                .Where(t => t.Date <= targetDate.ToDateTime(TimeOnly.MinValue)) 
                .OrderBy(t => t.Date)
                .ToList();
            return transactions.Sum(t => t.AmountEur);

        }
    }
}
