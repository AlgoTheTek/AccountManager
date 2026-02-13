using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Interfaces;
using AccountManager.Models;
using AccountManager.Services;

namespace AccountManager.Repositories
{
    public class CsvTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions;

        public CsvTransactionRepository(string csvPath)
        {
            var parser = new CsvParserFactory();
            _transactions = parser.ParseAccountCsv(csvPath).ToList();
        }

        public IEnumerable<Transaction> GetAll() => _transactions;

       /* public IEnumerable<Transaction> FindByDateRange(DateTime start, DateTime end)
            => _transactions.Where(t => t.Date >= start && t.Date <= end);*/
        public IEnumerable<Transaction> FindByDateRange(DateTime start, DateTime end)
            => _transactions.Where(t => t.Date.Date >= start.Date.Date && t.Date.Date.Date <= end.Date);
    }
}
