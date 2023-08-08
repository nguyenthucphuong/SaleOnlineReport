using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SaleOnline.Models;
using System.Diagnostics;

namespace SaleOnline.Areas.User.Controllers
{

    [Area("User")]
    [Route("user")]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly SaleOnline1Context? _dbContext;
        public HomeController(ILogger<HomeController> logger, SaleOnline1Context dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (!Request.Cookies.TryGetValue("RoleName", out var roleName))
            {
                return Redirect("/taikhoan/dangxuat");
            }

            if (roleName != "Admin" && roleName != "User")
            {
                 return Redirect("/taikhoan/dangxuat");
            }

            if (_dbContext == null)
            {
                return NotFound();
            }
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}


