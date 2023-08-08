using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using System.Data.SqlTypes;
using X.PagedList;

namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("khuyenmai")]
    public class PromotionsController : Controller
    {
        private readonly SaleOnline1Context _context;
        public PromotionsController(SaleOnline1Context context)
        {
            _context = context;
        }
        [Route("danhdach")]
        public IActionResult Index(string? filter, int page = 1, int pageSize = 5)
        {
            var query = _context.Promotions.Where(c => string.IsNullOrEmpty(filter) || c.Filter.Contains(filter!));
            page = page < 1 ? 1 : page;
            var promotions = query.ToPagedList(page, pageSize);

           
            if (promotions.PageNumber != 1 && page > promotions.PageCount)
            {
                page = promotions.PageCount;
                promotions = query.ToPagedList(page, pageSize);
            }

            ViewBag.TotalPages = promotions.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = promotions.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = promotions.TotalItemCount;

            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;

            ViewBag.TickId = TempData["TickId"];
            return View(promotions);
        }
        [HttpPost]
        public IActionResult StatusUpdate(Guid promotionId)
        {
            var promotion = _context.Promotions.FirstOrDefault(p => p.PromotionId == promotionId);

            if (promotion != null)
            {
                promotion.IsActive = !promotion.IsActive;
                _context.Promotions.Update(promotion);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            TempData["TickId"] = promotion?.PromotionId;
            return RedirectToAction("Index", new { page, pageSize });
        }

        [Route("xoa")]
        public IActionResult Xoa(Guid promotionId)
        {
            var item = _context.Promotions.FirstOrDefault(k => k.PromotionId == promotionId);

            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            if (item != null)
            {
                if (item.IsActive == false)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    TempData["Message"] = "Xóa Khuyến mãi thành công!";
                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    StatusUpdate(promotionId);
                    TempData["Message"] = "Khuyến mãi đã được Tạm khóa trước khi xóa!";
                }
            }
            return RedirectToAction("Index", new { page, pageSize });
        }
      
        [HttpPost]
        [Route("them")]
        public IActionResult Them(Guid? userId, decimal? discount, DateTime? startDate, DateTime? endDate, string? kichHoat)
        {
            var users = _context.Users.Where(u => u.IsActive == true).ToList();
            ViewBag.Users = users;
            ViewBag.UserIds = users.ToDictionary(u => u.UserName, u => u.UserId);

            if (discount.HasValue)
            {
                Promotion promotion = new Promotion(userId, discount ?? 0, startDate, endDate, kichHoat == "on" ? true : false);
                _context.Promotions.Add(promotion);
                _context.SaveChanges();
                // Thêm ID vào Filter để search
                promotion.Filter = promotion.Filter + " " + promotion.PromotionId;
                _context.SaveChanges();
                TempData["Message"] = "Thêm Khuyến mãi mới thành công!";
                
                TempData["TickId"] = promotion.PromotionId;

                int pageSize = (int)TempData["PageSize"]!;
                // Tìm vị trí của dòng mới trong danh sách
                var allPromotions = _context.Promotions.OrderBy(c => c.PromotionId).ToList();
                var newPromotionIndex = allPromotions.FindIndex(c => c.PromotionId == promotion.PromotionId);
                // Tính trang chứa dòng mới
                var newPromotionPage = (int)Math.Ceiling((newPromotionIndex + 1) / (double)pageSize);
                return RedirectToAction("Index", new { page = newPromotionPage, pageSize });
            }
            else
            {
                Promotion promotion = new Promotion();
                return View(promotion);
            }
        }
 
        [Route("capnhat")]
        public IActionResult CapNhat(Guid promotionId)
        {
            var users = _context.Users.Where(u => u.IsActive == true).ToList();
            ViewBag.Users = users;
            ViewBag.UserIds = users.ToDictionary(u => u.UserName, u => u.UserId);
            //ViewBag.UserIds = _context.Users.Where(u => u.IsActive == true).Select(u => u.UserId).ToList();

            var promotion = _context.Promotions.FirstOrDefault(k => k.PromotionId == promotionId);
            if (promotion == null)
            {
                return NotFound();
            }
            TempData["SelectedUserName"] = users.FirstOrDefault(u => u.UserId == promotion.UserId)?.UserName;

            return View(promotion);
        }
        [HttpPost]
        [Route("capnhat")]
        public IActionResult CapNhat(Guid? promotionId, Guid? userId, decimal? discount, DateTime? startDate, DateTime? endDate, string? kichHoat)
        {
            if (!promotionId.HasValue)
            {
                return RedirectToAction("Index");
            }
            var promotion = _context.Promotions.FirstOrDefault(k => k.PromotionId == promotionId.Value);
            if (promotion != null)
            {
                if (discount.HasValue)
                {
                    promotion.Discount = discount;
                    promotion.UserId = userId;
                    promotion.StartDate = startDate;
                    promotion.EndDate = endDate;
                    promotion.Filter = promotion.Discount.ToString() + " " + promotion.PromotionId;
                    promotion.IsActive = kichHoat == "on";
                    _context.Promotions.Update(promotion);
                    _context.SaveChanges();
                    TempData["Message"] = "Cập nhật Khuyến mãi thành công!";
                    int page = (int)TempData["page"]!;
                    int pageSize = (int)TempData["PageSize"]!;
                    TempData["TickId"] = promotion.PromotionId;

                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    return View(promotion);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
