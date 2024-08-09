using CashFlowControl.Core.Entities;
using CashFlowControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashFlowControl.Application.Services
{
    public class DailySummaryService
    {
        private readonly CashFlowContext _context;

        public DailySummaryService(CashFlowContext context)
        {
            _context = context;
        }

        public async Task<DailySummary> GetDailySummary(DateTime date)
        {
<<<<<<< HEAD
            // Check if we're asking for the summary of the previous day (D-1)
            var previousDay = DateTime.Today.AddDays(-1);

            if (date.Date == previousDay)
            {
                // Try to get the cached summary for the previous day
                var cachedSummary = await _context.DailySummaries
                    .FirstOrDefaultAsync(ds => ds.Date == previousDay);

                if (cachedSummary != null)
                {
                    return cachedSummary;
                }

                // If no cached summary, calculate from scratch
                return await CalculateAndCacheDailySummary(date);
            }
            else
            {
                // For any day other than D-1, always calculate (do not cache)
                return await CalculateDailySummary(date);
            }
        }

        private async Task<DailySummary> CalculateDailySummary(DateTime date)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Date.Date <= date.Date)
                .ToListAsync();
=======
            // Check if we have a cached summary for the previous day (D-1)
            var previousDay = date.Date.AddDays(-1);
            var cachedSummary = await _context.DailySummaries
                .FirstOrDefaultAsync(ds => ds.Date == previousDay);
>>>>>>> 9d1ab90e6ec3aca64a990be0e41b98e629779e8c

            if (cachedSummary != null)
            {
                // If we have a cached summary, use it as a starting point
                var todayTransactions = await _context.Transactions
                    .Where(t => t.Date.Date == date.Date)
                    .ToListAsync();

                var todayCredits = todayTransactions.Where(t => t.IsCredit).Sum(t => t.Amount);
                var todayDebits = todayTransactions.Where(t => !t.IsCredit).Sum(t => t.Amount);

                return new DailySummary
                {
                    Date = date,
                    TotalCredits = cachedSummary.TotalCredits + todayCredits,
                    TotalDebits = cachedSummary.TotalDebits + todayDebits,
                    Balance = cachedSummary.Balance + todayCredits - todayDebits
                };
            }
            else
            {
                // If no cached summary, calculate from scratch
                var transactions = await _context.Transactions
                    .Where(t => t.Date.Date <= date.Date)
                    .ToListAsync();

                var totalCredits = transactions.Where(t => t.IsCredit).Sum(t => t.Amount);
                var totalDebits = transactions.Where(t => !t.IsCredit).Sum(t => t.Amount);
                var balance = totalCredits - totalDebits;

                var summary = new DailySummary
                {
                    Date = date,
                    TotalCredits = totalCredits,
                    TotalDebits = totalDebits,
                    Balance = balance
                };

                // Cache the summary for the current day
                await CacheDailySummary(summary);

                return summary;
            }
        }

        private async Task CacheDailySummary(DailySummary summary)
        {
            _context.DailySummaries.Add(summary);
            await _context.SaveChangesAsync();
        }

        private async Task<DailySummary> CalculateAndCacheDailySummary(DateTime date)
        {
            var summary = await CalculateDailySummary(date);

            // Cache the summary for the current day (D-1)
            await CacheDailySummary(summary);

            return summary;
        }

        private async Task CacheDailySummary(DailySummary summary)
        {
            _context.DailySummaries.Add(summary);
            await _context.SaveChangesAsync();
        }
    }
}
