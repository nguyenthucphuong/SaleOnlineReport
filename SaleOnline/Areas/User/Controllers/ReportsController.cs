using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleOnline.Models;

namespace SaleOnline.Areas.User.Controllers
{
    //[Authorize(Roles = "Admin,User")]
    [Area("User")]
    [Route("baocao")]
    public class ReportsController : Controller
    {
        private readonly SaleOnline1Context _context;
        public ReportsController(SaleOnline1Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
       
        [Route("chitiet-sanpham")]
        public IActionResult DailyProductDetails(DateTime? date)
        {
            if (!date.HasValue)
            {
                date = DateTime.Today;
            }
            var orderDetails = _context.OrderItems
                .Where(oi => oi.OrderItemDatetime.Date == date.Value.Date)
                .Select(oi => new
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.ProductName,
                    ProductImage = oi.Product.ProductImage,
                    Quantity = oi.Quantity,
                    Total = oi.Price * oi.Quantity
                })
                .ToList();
            ViewBag.OrderDetails = orderDetails;
            ViewBag.Total = orderDetails.Sum(od => od.Total);
            ViewBag.SelectedDate = date.Value.ToString("yyyy-MM-dd");
            return View();
        }




        [Route("lichsu-donhang")]
        public IActionResult OrderHistory()
        {
            //var orders = _context.Orders.ToList();
            var orders = _context.Orders
       .OrderByDescending(o => o.OrderDatetime) // Sắp xếp theo ngày đặt hàng giảm dần
       .ToList();
            ViewBag.Statuses = orders.ToDictionary(o => o.OrderId, o => o.OrderStatus.OrderStatusName);
            ViewBag.OrderStatuses = _context.OrderStatuses.ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(Guid orderId, Guid orderStatusId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                TempData["Message"] = "Cập nhật trạng thái thành công!";
                order.OrderStatusId = orderStatusId;
                _context.SaveChanges();

            }
            return RedirectToAction("OrderHistory", "Reports");
        }

        [Route("chitiet-donhang")]
 
        public IActionResult OrderDetails(Guid orderid)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Promotion)
                .FirstOrDefault(o => o.OrderId == orderid);

            if (order == null)
            {
                return NotFound();
            }

            var discounts = new Dictionary<Guid, decimal>();
            foreach (var item in order.OrderItems)
            {
                if (item.Product.Promotion != null && item.Product.Promotion.Discount.HasValue)
                {
                    discounts[item.ProductId] = item.Product.Promotion.Discount.Value;
                }
            }
            ViewBag.Discounts = discounts;
            var orderItems = _context.OrderItems.Where(oi => oi.OrderId == orderid).ToList();
            ViewBag.OrderItems = orderItems;
            return View(order);
        }

    }
}

//[Route("chitiet-sanpham")]
//public IActionResult DailyProductDetails()
//{
//    var date = DateTime.Today;
//    var orderDetails = _context.OrderItems
//        .Where(oi => oi.OrderItemDatetime.Date == date)
//        .Select(oi => new
//        {
//            ProductId = oi.ProductId,
//            ProductName = oi.Product.ProductName,
//            ProductImage = oi.Product.ProductImage,
//            Quantity = oi.Quantity,
//            Total = oi.Price * oi.Quantity
//        })
//        .ToList();

//    ViewBag.OrderDetails = orderDetails;
//    ViewBag.Total = orderDetails.Sum(od => od.Total);

//    return View();
//}



//public IActionResult OrderDetails(Guid orderid)
//{
//    var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderid);
//    if (order == null)
//    {
//        return NotFound();
//    }

//    var orderItems = _context.OrderItems.Where(oi => oi.OrderId == orderid).ToList();
//    ViewBag.OrderItems = orderItems;

//    return View(order);
//}
