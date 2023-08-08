using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using X.PagedList;
namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("vaitro")]
    public class RolesController : Controller
    {
        private readonly SaleOnline1Context _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(SaleOnline1Context context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        [Route("danhdach")]

        public IActionResult Index(string? search, int page = 1, int pageSize = 5)
        {
            var query = _context.Roles.Where(c => string.IsNullOrEmpty(search) || c.Filter.Contains(search!));
            page = page < 1 ? 1 : page;
            var roles = query.ToPagedList(page, pageSize);
            if (roles.PageNumber != 1 && page > roles.PageCount)
            {
                page = roles.PageCount;
                roles = query.ToPagedList(page, pageSize);
            }
            ViewBag.TotalPages = roles.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = roles.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = roles.TotalItemCount;
            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;
            return View(roles);
        }
        [HttpPost]
        [Route("status")]
        public IActionResult StatusUpdate(Guid roleId)
        {
            var role = _context.Roles.FirstOrDefault(k => k.RoleId == roleId);

            if (role != null)
            {
                role.IsActive = !role.IsActive;
                _context.Roles.Update(role);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;

            TempData["TickId"] = role?.RoleId;
            return RedirectToAction("Index", new { page, pageSize });
        }
    }
}
