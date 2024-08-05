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
            // Check if we have a cached summary for the previous day (D-1)
            var previousDay = date.Date.AddDays(-1);
            var cachedSummary = await _context.DailySummaries
                .FirstOrDefaultAsync(ds => ds.Date == previousDay);

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
    }
}
