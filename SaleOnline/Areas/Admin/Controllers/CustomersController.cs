using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using X.PagedList;

namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("khachhang")]
    public class CustomersController : Controller
    {
        private readonly SaleOnline1Context _context;

        public CustomersController(SaleOnline1Context context)
        {
            _context = context;
        }
        [Route("danhdach")]
        public IActionResult Index(string? search, int page = 1, int pageSize = 5)
        {
            var query = _context.Customers.Where(c => string.IsNullOrEmpty(search) || c.Filter.Contains(search!));
            page = page < 1 ? 1 : page;
            var customers = query.ToPagedList(page, pageSize);
            if (customers.PageNumber != 1 && page > customers.PageCount)
            {
                page = customers.PageCount;
                customers = query.ToPagedList(page, pageSize);
            }
            ViewBag.TotalPages = customers.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = customers.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = customers.TotalItemCount;
            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;
            ViewBag.TickId = TempData["TickId"];
            return View(customers);
        }
        [HttpPost]
        public IActionResult StatusUpdate(Guid customerId)
        {
            var customer = _context.Customers.FirstOrDefault(k => k.CustomerId == customerId);

            if (customer != null)
            {
                customer.IsActive = !customer.IsActive;
                _context.Customers.Update(customer);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;

            TempData["TickId"] = customer?.CustomerId;
            return RedirectToAction("Index", new { page, pageSize });
        }
        [Route("xoa")]
        public IActionResult Xoa(Guid customerId)
        {
            var item = _context.Customers.FirstOrDefault(k => k.CustomerId == customerId);

            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            if (item != null)
            {
                if (item.IsActive == false)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    TempData["Message"] = "Bạn đã xóa thành công!";
                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    StatusUpdate(customerId);
                    TempData["Message"] = "Tài khoản đã được Tạm khóa trước khi xóa!";
                }
            }
            return RedirectToAction("Index", new { page, pageSize });
        }

    }
}
