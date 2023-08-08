using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using System.Drawing.Printing;
using X.PagedList;
using Common.Utilities;
namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("danhmuc")]
    public class CategoriesController : Controller
    {
        private readonly SaleOnline1Context _context;
        public CategoriesController(SaleOnline1Context context)
        {
            _context = context;
        }
        [Route("danhdach")]
       
        public IActionResult Index(string? search, int page = 1, int pageSize = 5)
        {

            var query = _context.Categories.Where(c => string.IsNullOrEmpty(search) || c.Filter.Contains(search!));
            page = page < 1 ? 1 : page;
            var categories = query.ToPagedList(page, pageSize);
            if (categories.PageNumber != 1 && page > categories.PageCount)
            {
                page = categories.PageCount;
                categories = query.ToPagedList(page, pageSize);
            }

            ViewBag.TotalPages = categories.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = categories.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = categories.TotalItemCount;

            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;
            ViewBag.TickId = TempData["TickId"];
            return View(categories);
        }



        [HttpPost]
        [Route("status")]
        public IActionResult StatusUpdate(Guid categoryId)
        {
            var category = _context.Categories.FirstOrDefault(k => k.CategoryId == categoryId);

            if (category != null)
            {
                category.IsActive = !category.IsActive;
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;

            TempData["TickId"] = category?.CategoryId;
            return RedirectToAction("Index", new { page, pageSize });
        }

        [Route("xoa")]
        public IActionResult Xoa(Guid categoryId)
        {
            var item = _context.Categories.FirstOrDefault(k => k.CategoryId == categoryId);

            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            if (item != null)
            {
                if (item.IsActive == false)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    TempData["Message"] = "Xóa Danh mục thành công!";
                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    StatusUpdate(categoryId);
                    TempData["Message"] = "Danh mục đã được Tạm khóa trước khi xóa!";
                }
            }
            return RedirectToAction("Index", new { page, pageSize });
        }
       
        [HttpPost]
        [Route("them")]

        public IActionResult Them(string categoryName, string filter, string? kichHoat)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                Category category = new Category(categoryName, kichHoat == "on" ? true : false);
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["Message"] = "Thêm danh mục mới thành công!";

                TempData["TickId"] = category.CategoryId;

                int pageSize = (int)TempData["PageSize"]!;
                // Tìm vị trí của danh mục mới trong danh sách tất cả các danh mục
                var allCategories = _context.Categories.OrderBy(c => c.CategoryId).ToList();
                var newCategoryIndex = allCategories.FindIndex(c => c.CategoryId == category.CategoryId);
                // Tính trang chứa danh mục mới
                var newCategoryPage = (int)Math.Ceiling((newCategoryIndex + 1) / (double)pageSize);
                return RedirectToAction("Index", new { page = newCategoryPage, pageSize });
            }
            else
            {
                Category category = new Category();
                return View(category);
            }
        }

        [HttpGet]
        [Route("capnhat")]
        public IActionResult CapNhat(Guid categoryId)
        {
            var category = _context.Categories.FirstOrDefault(k => k.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [Route("capnhat")]
        public IActionResult CapNhat(Guid? categoryId, string categoryName, string? kichHoat)
        {
            if (!categoryId.HasValue)
            {
                return RedirectToAction("Index");
            }
            var category = _context.Categories.FirstOrDefault(k => k.CategoryId == categoryId.Value);
            if (category != null)
            {
                if (!string.IsNullOrEmpty(categoryName))
                {
                    category.CategoryName = categoryName.Trim();
                    category.Filter = Utility.ConvertToUnsign(category.CategoryName.ToLower()) + " " + category.CategoryName.ToLower();
                    category.IsActive = kichHoat == "on";
                    _context.Categories.Update(category);
                    _context.SaveChanges();
                    TempData["Message"] = "Cập nhật danh mục thành công!";
                    int page = (int)TempData["page"]!;
                    int pageSize = (int)TempData["PageSize"]!;

                    TempData["TickId"] = category.CategoryId;

                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    return View(category);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
