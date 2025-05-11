namespace FinanceTrackerAPI.DataModel
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int CategoryId { get; set; }   
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }

    }
}
