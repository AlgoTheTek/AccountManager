
using AccountManager.Interfaces;
using AccountManager.Models;
using AccountManager.Repositories;
using AccountManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;


// See https://aka.ms/new-console-template for more information


var builder = Host.CreateApplicationBuilder(args);

// DI
builder.Services.AddScoped<ITransactionRepository>(sp =>
    new CsvTransactionRepository("account_20230228.csv"));
builder.Services.AddScoped<IAccountService, AccountService>();

var host = builder.Build();
var service = host.Services.GetRequiredService<IAccountService>();

// Tests
Console.WriteLine("=== GESTION DE COMPTE ===");
Console.WriteLine($"Balance au 28/02/2023: {service.GetBalanceAtDate(new DateTime(2023, 2, 28)).BalanceEur:F2} EUR");
var tx2022 = service.GetTransactions(
    new DateTime(2022, 1, 1),
    new DateTime(2022, 02, 28));
Console.WriteLine($"Transactions au 28/02/2022: {tx2022.Count()}");
// Pour obtenir la somme des AmountEur  :
var sommeAmountEur = tx2022.Sum(t => t.AmountEur);
Console.WriteLine($"Somme Des transactions (du 01/01/2022 au  28/02/2022) : {sommeAmountEur:F2} EUR");


//Console.WriteLine($"USD -> EUR: {service.GetExchangeRate("USD", "EUR")}");

// === TEST VALEUR COMPTE 01/01/2022 - 01/03/2023 ===
Console.WriteLine("\n=== TEST PÉRIODE 01/01/2022 - 01/03/2023 ===");

var startDate = new DateTime(2022, 1, 1);
var endDate = new DateTime(2023, 3, 1);

var periodTx = service.GetTransactions(startDate, endDate);
var periodBalance = periodTx.Sum(t => t.AmountEur);

Console.WriteLine($"Période: {startDate:dd/MM/yyyy} → {endDate:dd/MM/yyyy}");
Console.WriteLine($"Nombre transactions: {periodTx.Count()}");
Console.WriteLine($"Balance totale EUR: {periodBalance:F2} €");

Console.WriteLine("\nTop 3 catégories débits (période):");
var debitsByCategory = periodTx
    .Where(t => t.AmountEur < 0)
    .GroupBy(t => t.Category)
    .Select(g => new { Category = g.Key, Total = g.Sum(t => t.AmountEur) })
    .OrderByDescending(g => g.Total)
    .Take(3);

foreach (var cat in debitsByCategory)
{
    Console.WriteLine($"  {cat.Category}: {cat.Total:F2} €");
}