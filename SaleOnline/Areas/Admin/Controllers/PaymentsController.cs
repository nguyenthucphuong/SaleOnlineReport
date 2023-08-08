using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using X.PagedList;

namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("thanhtoan")]
    public class PaymentsController : Controller
    {
        private readonly SaleOnline1Context _context;
        public PaymentsController(SaleOnline1Context context)
        {
            _context = context;
        }
        [Route("danhdach")]
        public IActionResult Index(string? search, int page = 1, int pageSize = 5)
        {
            var query = _context.Payments.Where(c => string.IsNullOrEmpty(search) || c.Filter.Contains(search!));
            page = page < 1 ? 1 : page;
            var payments = query.ToPagedList(page, pageSize);
            if (payments.PageNumber != 1 && page > payments.PageCount)
            {
                page = payments.PageCount;
                payments = query.ToPagedList(page, pageSize);
            }
            ViewBag.TotalPages = payments.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = payments.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = payments.TotalItemCount;

            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;
            ViewBag.TickId = TempData["TickId"];
            return View(payments);
        }

        [HttpPost]
         public IActionResult StatusUpdate(Guid paymentId)
        {
            var payment = _context.Payments.FirstOrDefault(k => k.PaymentId == paymentId);

            if (payment != null)
            {
                payment.IsActive = !payment.IsActive;
                _context.Payments.Update(payment);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            TempData["TickId"] = payment?.PaymentId;
            return RedirectToAction("Index", new { page, pageSize });
        }

    }
}
