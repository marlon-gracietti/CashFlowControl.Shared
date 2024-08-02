using System;

namespace CashFlowControl.Core.Entities
{
    public class DailySummary
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal Balance { get; set; }
    }
}
