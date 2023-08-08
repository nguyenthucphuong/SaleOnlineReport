namespace SaleOnline.Models
{
    public class CartCookie
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }
    }
}
