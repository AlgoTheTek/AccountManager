using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Models
{
    public record Transaction(
        DateTime Date,
        decimal AmountEur,
        string Currency,
        string Category,
        decimal OriginalAmount
    );


}
