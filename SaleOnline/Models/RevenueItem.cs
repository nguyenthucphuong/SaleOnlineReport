namespace SaleOnline.Models
{
    public class RevenueItem
    {
        public DateTime Date { get; internal set; }
        public decimal Revenue { get; internal set; }
        public decimal Refund { get; internal set; }
    }
}