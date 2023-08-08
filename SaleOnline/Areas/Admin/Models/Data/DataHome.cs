using SaleOnline.Models;

namespace SaleOnline.Areas.Admin.Models.Data
{
    public class DataHome
    {
        public List<OrderItem>? DsOrderItems;

        public List<Category>? DsCategories { get; set; }
        public List<OrderItem>? DsOrders { get; internal set; }
        public List<Payment>? DsPayments { get; internal set; }
        public List<Product>? DsProducts { get; internal set; }
        public List<Promotion>? DsPromotions { get; internal set; }
        public List<Role>? DsRoles { get; set; }
        //public List<User>? DsUsers { get; set; }
        public List<SaleOnline.Models.User>? DsUsers { get; set; }
        public List<OrderStatus>? DsOrderStatuses { get; internal set; }
    }
}
