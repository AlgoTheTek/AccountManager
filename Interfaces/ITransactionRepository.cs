using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Models;

namespace AccountManager.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> FindByDateRange(DateTime start, DateTime end);
    }

}
