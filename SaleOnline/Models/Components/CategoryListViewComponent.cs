using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SaleOnline.Models.Components
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly SaleOnline1Context _context;

        public CategoryListViewComponent(SaleOnline1Context context)
        {
            _context = context;
        }
        // Cần chờ kết quả của ToList
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
    }
}
