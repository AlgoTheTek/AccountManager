using AccountManager.Interfaces;
using AccountManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Services
{
    public class AccountService : IAccountService
    {
        private readonly ITransactionRepository _repo;

        public AccountService(ITransactionRepository repo) => _repo = repo;

        public AccountState GetBalanceAtDate(DateTime date)
        {
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);
            var tx = _repo.FindByDateRange(DateTime.MinValue.Date, endOfDay.Date);
            var balance = tx.Sum(t => t.AmountEur);
            return new AccountState(date, balance);
        }

        public IEnumerable<Transaction> GetTransactions(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var start = fromDate ?? DateTime.MinValue;
            var end = toDate ?? DateTime.MaxValue;
            return _repo.FindByDateRange(start, end);
        }

        public IEnumerable<Transaction> GetAllTransactions() => _repo.GetAll();

        public decimal GetExchangeRate(string fromCurrency, string toCurrency)
        {
            // Simplifié - utilise taux fixes du parser
            return fromCurrency switch
            {
                "USD" when toCurrency == "EUR" => 1.445m,
                "JPY" when toCurrency == "EUR" => 0.482m,
                _ => 1m
            };
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
