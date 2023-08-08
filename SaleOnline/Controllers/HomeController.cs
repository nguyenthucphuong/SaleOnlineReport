using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using SaleOnline.Models;
using Microsoft.AspNetCore.Http;

namespace SaleOnline.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SaleOnline1Context _context;
        public HomeController(ILogger<HomeController> logger, SaleOnline1Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var newProducts = _context.Products.Where(p => p.IsActive == true && p.IsNew == true).ToList();
            var bestSellingProducts = _context.Products.Where(p => p.IsActive == true && p.IsSale == true).ToList();
            var promotionProducts = _context.Products.Where(p => p.IsActive == true && p.IsPro == true).ToList();

            ViewBag.NewProducts = newProducts;
            ViewBag.BestSellingProducts = bestSellingProducts;
            ViewBag.PromotionProducts = promotionProducts;
            //if (Request.Cookies.TryGetValue("Cart", out var cartJson))
            //{
            //    var cartCookie = JsonConvert.DeserializeObject<List<CartCookie>>(cartJson) ?? new List<CartCookie>();
            //    var totalQuantity = cartCookie.Sum(c => c.Quantity);
            //    ViewBag.TotalQuantity = totalQuantity;
            //}



            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}


