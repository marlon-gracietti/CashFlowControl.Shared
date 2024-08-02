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
            var transactions = await _context.Transactions
                .Where(t => t.Date.Date == date.Date)
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
    }
}
