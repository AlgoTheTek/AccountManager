using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AccountManager.Models;

namespace AccountManager.Services
{
    public class CsvParserFactory
    {
        public IEnumerable<Transaction> ParseAccountCsv(string csvPath)
        {

            var lines = File.ReadAllLines(csvPath);
            var rates = ExtractExchangeRates(string.Join("\n", lines.Take(3)));

            var transactions = new List<Transaction>();

            // Parser transactions ,skip header, start at lignes 4
            foreach (var line in lines.Skip(4))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains(';')) continue;

                var parts = line.Split(';');
                if (parts.Length < 4) continue;

                try
                {
                    var date = DateTime.ParseExact(parts[0].Trim(), "dd/MM/yyyy", null);

                    decimal.TryParse(parts[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var amount);
                    var currency = parts[2].Trim();
                    var category = parts[3].Trim();

                    var amountEur = ConvertToEur(amount, currency, rates);

                    transactions.Add(new Transaction(date, amountEur, currency, category, amount));
                }
                catch (Exception e) { 
                    Console.WriteLine(e.Message.ToString());
                }
            }

            return transactions.OrderBy(t => t.Date);
        }

        private static decimal ConvertToEur(decimal amount, string currency, Dictionary<(string, string), decimal> rates)
        {
            var key = (currency, "EUR");
            return rates.TryGetValue(key, out var rate) ? amount * rate : amount;
        }

        private static Dictionary<(string, string), decimal> ExtractExchangeRates(string header)
        {
            var rates = new Dictionary<(string, string), decimal>
            {
                [("EUR", "EUR")] = 1m,
                [("USD", "EUR")] = 1.445m,
                [("JPY", "EUR")] = 0.482m,
                [("EUR", "USD")] = 0.692m,
                [("EUR", "JPY")] = 0.482m
            };

            // Parsing français (virgule) + anglais (point)
            var matches = Regex.Matches(header, @"([A-Z]{3})/([A-Z]{3})\s*:\s*([\d.,]+)");
            foreach (Match m in matches)
            {
                var rateStr = m.Groups[3].Value.Replace(',', '.');  // Virgule → point
                if (decimal.TryParse(rateStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                {
                    rates[(m.Groups[1].Value, m.Groups[2].Value)] = rate;
                }
            }

            return rates;
        }
    }

}





