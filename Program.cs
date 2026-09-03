
using AccountManager.Interfaces;
using AccountManager.Models;
using AccountManager.Repositories;
using AccountManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// See https://aka.ms/new-console-template for more information
Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

// ---------- Configuration ----------
// Exemple : appsettings.json avec une section "TransactionFile" { "FileName": "transactions.csv" }
var configuration = builder.Configuration;

// ---------- Dependency Injection ----------
builder.Services.AddScoped<ITransactionRepository>(sp =>
{
    var fileName = configuration["TransactionFile:FileName"]
                   ?? throw new InvalidOperationException("Configuration 'TransactionFile:FileName' manquante.");

    return new CsvTransactionRepository(fileName);
});

builder.Services.AddScoped<IAccountService, AccountService>();

// ---------- Build host & resolve services ----------
var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var accountService = host.Services.GetRequiredService<IAccountService>();

logger.LogInformation("Démarrage des tests de gestion de compte.");

// ---------- Exécution des scénarios de test ----------
try
{
    RunBalanceAtDateScenario(accountService, logger);
    RunTransactionsSummaryScenario(accountService, logger);
    RunPeriodAnalysisScenario(accountService, logger);
}
catch (Exception ex)
{
    logger.LogError(ex, "Erreur lors de l'exécution des scénarios de test.");
    throw;
}

logger.LogInformation("Fin des tests de gestion de compte.");

// ---------- Scénarios ----------

static void RunBalanceAtDateScenario(IAccountService service, ILogger logger)
{
    logger.LogInformation("=== GESTION DE COMPTE – Balance à une date ===");

    var targetDate = new DateTime(2023, 2, 28);
    var balance = service.GetBalanceAtDate(targetDate);

    Console.WriteLine("=== GESTION DE COMPTE ===");
    Console.WriteLine($"Balance au {targetDate:dd/MM/yyyy}: {balance.BalanceEur:F2} EUR");

    logger.LogInformation("Balance au {Date}: {Balance:F2} EUR", targetDate, balance.BalanceEur);
}

static void RunTransactionsSummaryScenario(IAccountService service, ILogger logger)
{
    logger.LogInformation("=== GESTION DE COMPTE – Résumé transactions 2022 ===");

    var startDate = new DateTime(2022, 1, 1);
    var endDate = new DateTime(2022, 2, 28);

    var transactions = service.GetTransactions(startDate, endDate).ToList();
    var sumAmountEur = transactions.Sum(t => t.AmountEur);

    Console.WriteLine($"Transactions du {startDate:dd/MM/yyyy} au {endDate:dd/MM/yyyy}: {transactions.Count}");
    Console.WriteLine($"Somme des transactions : {sumAmountEur:F2} EUR");

    logger.LogInformation(
        "Transactions du {Start} au {End}: {Count}, somme: {Sum:F2} EUR",
        startDate, endDate, transactions.Count, sumAmountEur);
}

static void RunPeriodAnalysisScenario(IAccountService service, ILogger logger)
{
    logger.LogInformation("=== TEST PÉRIODE 01/01/2022 – 01/03/2023 ===");

    var startDate = new DateTime(2022, 1, 1);
    var endDate = new DateTime(2023, 3, 1);

    var periodTx = service.GetTransactions(startDate, endDate).ToList();
    var periodBalance = periodTx.Sum(t => t.AmountEur);

    Console.WriteLine("\n=== TEST PÉRIODE 01/01/2022 – 01/03/2023 ===");
    Console.WriteLine($"Période: {startDate:dd/MM/yyyy} → {endDate:dd/MM/yyyy}");
    Console.WriteLine($"Nombre de transactions: {periodTx.Count}");
    Console.WriteLine($"Balance totale EUR: {periodBalance:F2} €");

    logger.LogInformation(
        "Période {Start}–{End}: {Count} transactions, balance: {Balance:F2} €",
        startDate, endDate, periodTx.Count, periodBalance);

    // Top 3 catégories de débits
    var debitsByCategory = periodTx
        .Where(t => t.AmountEur < 0)
        .GroupBy(t => t.Category)
        .Select(g => new { Category = g.Key, Total = g.Sum(t => t.AmountEur) })
        .OrderByDescending(g => g.Total)
        .Take(3)
        .ToList();

    Console.WriteLine("\nTop 3 catégories débits (période):");

    foreach (var cat in debitsByCategory)
    {
        Console.WriteLine($"  {cat.Category}: {cat.Total:F2} €");
        logger.LogInformation("Catégorie débit: {Category}, total: {Total:F2} €", cat.Category, cat.Total);
    }
}