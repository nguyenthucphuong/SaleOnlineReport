using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleOnline.Models;

namespace SaleOnline.Controllers
{
    public class DetailsController : Controller
    {
        private readonly SaleOnline1Context _context;
        public DetailsController(SaleOnline1Context context)
        {
            _context = context;
        }
      
        public IActionResult Index(Guid productId)
        {
            if (_context == null)
            {
                return NotFound();
            }
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
       
    }
}
