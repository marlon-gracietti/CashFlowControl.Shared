namespace CashFlowControl.Core.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public bool IsCredit { get; set; }
    }
}
