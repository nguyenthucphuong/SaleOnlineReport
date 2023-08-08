using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using X.PagedList;
namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("trangthai-donhang")]
    public class OrderStatusesController : Controller
    {
        private readonly SaleOnline1Context _context;
        public OrderStatusesController(SaleOnline1Context context)
        {
            _context = context;
        }
        [Route("danhdach")]
        public IActionResult Index(string? search, int page = 1, int pageSize = 5)
        {
            var query = _context.OrderStatuses.Where(c => string.IsNullOrEmpty(search) || c.Filter.Contains(search!));
            page = page < 1 ? 1 : page;
            var orderStatuses = query.ToPagedList(page, pageSize);
            if (orderStatuses.PageNumber != 1 && page > orderStatuses.PageCount)
            {
                page = orderStatuses.PageCount;
                orderStatuses = query.ToPagedList(page, pageSize);
            }
            ViewBag.TotalPages = orderStatuses.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = orderStatuses.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = orderStatuses.TotalItemCount;

            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;
            ViewBag.TickId = TempData["TickId"];
            return View(orderStatuses);
        }

        [HttpPost]
        public IActionResult StatusUpdate(Guid orderStatusId)
        {
            var orderStatus = _context.OrderStatuses.FirstOrDefault(k => k.OrderStatusId == orderStatusId);

            if (orderStatus != null)
            {
                orderStatus.IsActive = !orderStatus.IsActive;
                _context.OrderStatuses.Update(orderStatus);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            TempData["TickId"] = orderStatus?.OrderStatusId;
            return RedirectToAction("Index", new { page, pageSize });
        }

        [Route("xoa")]
        public IActionResult Xoa(Guid orderStatusId)
        {
            var item = _context.OrderStatuses.FirstOrDefault(k => k.OrderStatusId == orderStatusId);

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
                    StatusUpdate(orderStatusId);
                    TempData["Message"] = "Tài khoản đã được Tạm khóa trước khi xóa!";
                }
            }
            return RedirectToAction("Index", new { page, pageSize });
        }

    }
}
