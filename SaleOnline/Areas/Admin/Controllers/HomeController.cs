using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using SaleOnline.Areas.Admin.Models.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace SaleOnline.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("admin")]
    public class HomeController : Controller
    {
        private readonly SaleOnline1Context _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, SaleOnline1Context context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
          
            if (!Request.Cookies.TryGetValue("RoleName", out var roleName))
            {
                return Redirect("/taikhoan/dangxuat");
            }

            if (roleName != "Admin")
            {
                return Redirect("/taikhoan/dangxuat");
            }

            if (_context == null)
            {
                return NotFound();
            }
            return View();
        }

    }
}
