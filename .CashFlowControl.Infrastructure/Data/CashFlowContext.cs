using Microsoft.EntityFrameworkCore;
using CashFlowControl.Core.Entities;

namespace CashFlowControl.Infrastructure.Data
{
    public class CashFlowContext : DbContext
    {
        public CashFlowContext(DbContextOptions<CashFlowContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
