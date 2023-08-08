using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleOnline.Models;
using System.Linq;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("bao-cao")]
    public class ReportsController : BaseController
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

        [Route("danh-sach-don-hang")]

        public IActionResult OrderList(int page = 1, int pageSize = 5)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .ThenInclude(u => u.Customer)
                .Include(o => o.OrderStatus)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.User.Customer.FullName,
                    PhoneNumber = o.User.Customer.PhoneNumber,
                    Total = o.Total,
                    Status = o.OrderStatus.OrderStatusName,
                    OrderDate = o.OrderDatetime,
                    OrderStatusId = o.OrderStatusId,
                })
                .OrderByDescending(o => o.OrderDate) // Sắp xếp theo ngày đặt hàng giảm dần
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            foreach (var order in orders)
            {
                if (order.Status == "Đã hủy")
                {
                    order.Refund = order.Total;
                }
            }

            ViewBag.Statuses = orders.ToDictionary(o => o.OrderId, o => o.Status);
            ViewBag.OrderStatuses = _context.OrderStatuses.ToList();

            ViewBag.TotalRevenue = _context.Orders.Sum(o => o.Total);
            ViewBag.TotalCanceledRevenue = _context.Orders.Where(o => o.OrderStatus.OrderStatusName == "Đã hủy").Sum(o => o.Total);
            ViewBag.TotalRealRevenue = ViewBag.TotalRevenue - ViewBag.TotalCanceledRevenue;

            ViewBag.Page = page;
            ViewBag.PageCount = (int)Math.Ceiling((double)_context.Orders.Count() / pageSize);
            ViewBag.PageSize = pageSize;

            return View(orders);
        }


        [HttpPost]
        [Route("trangthai-donhang")]
        public IActionResult UpdateOrderStatus(Guid orderId, Guid orderStatusId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.OrderStatusId = orderStatusId;
            _context.SaveChanges();
            TempData["Message"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("OrderList");
        }

       
        [Route("chitiet-donhang/{id}")]
        public IActionResult OrderDetails(Guid id)
        {
            // Truy vấn đến bảng Orders trong cơ sở dữ liệu
            var order = _context.Orders
                // Bao gồm thông tin của User liên quan đến Order
                .Include(o => o.User)
                // Bao gồm thông tin của Customer liên quan đến User
                .ThenInclude(u => u.Customer)
                // Bao gồm thông tin của OrderItems liên quan đến Order
                .Include(o => o.OrderItems)
                // Bao gồm thông tin của Product liên quan đến mỗi OrderItem
                .ThenInclude(oi => oi.Product)
                // Bao gồm thông tin của Promotion liên quan đến mỗi Product
                .ThenInclude(p => p.Promotion)
                // Bao gồm thông tin của Promotion liên quan đến Order
                .Include(o => o.Promotion)
                // Lấy ra Order đầu tiên có OrderId bằng id truyền vào
                .FirstOrDefault(o => o.OrderId == id);
            // Kiểm tra xem order có tồn tại hay không
            if (order == null)
            {
                return NotFound();
            }
            // Chuyển đổi order thành đối tượng OrderViewModel
            var orderViewModel = new OrderViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.User?.Customer?.FullName ?? string.Empty,
                PhoneNumber = order.User?.Customer?.PhoneNumber ?? string.Empty,
                Total = order.Total,
                Discount = order.Promotion?.Discount ?? 0,
                Status = order.OrderStatus?.OrderStatusName ?? string.Empty,
                OrderDate = order.OrderDatetime,
                OrderStatusId = order.OrderStatusId,
                // Chuyển đổi danh sách OrderItems thành danh sách các đối tượng OrderItemViewModel
                OrderItems = order.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product?.ProductName ?? string.Empty,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Discount = oi.Product?.Promotion?.Discount ?? 0,
                    ProductImage = oi.Product?.ProductImage ?? string.Empty
                }).ToList() ?? new List<OrderItemViewModel>()
            };

            return View(orderViewModel);
        }

        [Route("doanh-thu-ngay")]
        public IActionResult Revenue(string reportType, string selectedValue, int page = 1, int pageSize = 5)
        {
            

            var totalOrders = _context.Orders.Count();
            var totalRevenue = _context.Orders.Sum(o => o.Total);
            ViewData["ReportType"] = reportType;
            DateTime? selectedDate = null;
            DateTime? selectedMonth = null;
            int? selectedQuarter = null;
            if (reportType == "date" && !string.IsNullOrEmpty(selectedValue))
            {
                selectedDate = DateTime.ParseExact(selectedValue, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else if (reportType == "month" && !string.IsNullOrEmpty(selectedValue))
            {
                selectedMonth = DateTime.ParseExact(selectedValue, "yyyy-MM", CultureInfo.InvariantCulture);
            }
            else if (reportType == "quarter" && !string.IsNullOrEmpty(selectedValue))
            {
                selectedQuarter = int.Parse(selectedValue);
            }

            var revenueQuery = _context.Orders.AsQueryable();
            var refundQuery = _context.Orders
                .Where(o => o.OrderStatus.OrderStatusName == "Đã hủy")
                .AsQueryable();

            if (selectedDate.HasValue)
            {
                revenueQuery = revenueQuery.Where(o => o.OrderDatetime.Date == selectedDate.Value.Date);
                refundQuery = refundQuery.Where(o => o.OrderDatetime.Date == selectedDate.Value.Date);
            }
            else if (selectedMonth.HasValue)
            {
                revenueQuery = revenueQuery.Where(o => o.OrderDatetime.Year == selectedMonth.Value.Year && o.OrderDatetime.Month == selectedMonth.Value.Month);
                refundQuery = refundQuery.Where(o => o.OrderDatetime.Year == selectedMonth.Value.Year && o.OrderDatetime.Month == selectedMonth.Value.Month);
            }
            else if (selectedQuarter.HasValue)
            {
                int quarterStartMonth = (selectedQuarter.Value - 1) * 3 + 1;
                int quarterEndMonth = quarterStartMonth + 2;
                revenueQuery = revenueQuery.Where(o => o.OrderDatetime.Month >= quarterStartMonth && o.OrderDatetime.Month <= quarterEndMonth);
                refundQuery = refundQuery.Where(o => o.OrderDatetime.Month >= quarterStartMonth && o.OrderDatetime.Month <= quarterEndMonth);
            }

            var revenue = revenueQuery
                .GroupBy(o => o.OrderDatetime.Date)
                .Select(g => new RevenueItem { Date = g.Key, Revenue = g.Sum(o => o.Total) })
                .ToList();

            var refund = refundQuery
                .GroupBy(o => o.OrderDatetime.Date)
                .Select(g => new RevenueItem { Date = g.Key, Refund = g.Sum(o => o.Total) })
                .ToList();

            var totalRefund = refund.Sum(r => r.Refund);

            var model = new RevenueViewModel
            {
                OrderDatetime = selectedDate,
                SelectedQuarter = selectedQuarter,
                Revenues = revenue.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Refunds = refund.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalRefund = totalRefund
            };
            ViewBag.Page = page;
            ViewBag.PageCount = (int)Math.Ceiling((double)revenue.Count / pageSize);
            ViewBag.PageSize = pageSize;
            return View(model);
        }

        [Route("doanh-thu-thang")]
        public IActionResult MonthlyRevenue()
        {
            var monthlyRevenue = _context.Orders
                .GroupBy(o => new { o.OrderDatetime.Year, o.OrderDatetime.Month })
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Revenue = g.Sum(o => o.Total) })
                .ToList();
            return View(monthlyRevenue);
        }
        [Route("doanh-thu-quy")]
        public IActionResult QuarterlyRevenue()
        {
            var quarterlyRevenue = _context.Orders
                .GroupBy(o => new { o.OrderDatetime.Year, Quarter = (o.OrderDatetime.Month - 1) / 3 + 1 })
                .Select(g => new { Year = g.Key.Year, Quarter = g.Key.Quarter, Revenue = g.Sum(o => o.Total) })
                .ToList();
            return View(quarterlyRevenue);
        }

    }
}
