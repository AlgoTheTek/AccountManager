using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Models
{
    public class ExchangeRateConfig
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
}
