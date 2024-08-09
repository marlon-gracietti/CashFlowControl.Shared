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

            var totalCredits = transactions.Where(t => t.IsCredit).Sum(t => t.Amount);
            var totalDebits = transactions.Where(t => !t.IsCredit).Sum(t => t.Amount);
            var balance = totalCredits - totalDebits;

            return new DailySummary
            {
                Date = date,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits,
                Balance = balance
            };
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
