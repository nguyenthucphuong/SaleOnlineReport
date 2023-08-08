namespace SaleOnline.Models
{
    public class RevenueViewModel
    {
        public DateTime? OrderDatetime { get; set; }
        public int? SelectedQuarter { get; set; }
        public List<RevenueItem> Revenues { get; set; }
        public List<RevenueItem> Refunds { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalRefund { get; set; }
        public string Filter { get; set; }
        public RevenueViewModel()
        {
            Revenues = new List<RevenueItem>();
            Refunds = new List<RevenueItem>();
            Filter = OrderDatetime?.ToString() ?? "";
        }
    }

}


//public class DailyRevenueItem
//{
//    public DateTime Date { get; set; }
//    public decimal Revenue { get; set; }
//}
