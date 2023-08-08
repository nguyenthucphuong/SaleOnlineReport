using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleOnline.Models;
using X.PagedList;
using Common.Utilities;
namespace SaleOnline.Controllers
{
    public class ShopsController : Controller
    {
        private readonly SaleOnline1Context _context;
        public ShopsController(SaleOnline1Context context)
        {
            _context = context;
        }

        //public IActionResult FilterByCategory(string categoryName)
        //{
        //    var products = _context.Products
        //        .Include(p => p.Category)
        //        .Where(p => p.IsActive && p.Category.CategoryName == categoryName)
        //        .ToList();
        //    return PartialView("_ProductList", products);
        //}


        //public IActionResult Index(string? filter, string sorting, int? minPrice, int? maxPrice, int page = 1, int pageSize = 6)
        public IActionResult Index(string? filter, string sorting, int? minPrice, int? maxPrice, string? categoryName, int page = 1, int pageSize = 6)
        {
            var query = _context.Products.Where(p => p.IsActive == true && (string.IsNullOrEmpty(filter) || p.Filter.Contains(filter!)));

            if (!string.IsNullOrEmpty(categoryName))
            {
                //query = query.Where(p => p.Category.CategoryName == categoryName);
                query = query.Where(p => p.Category != null && p.Category.CategoryName == categoryName);

            }

            if (sorting == "isPro")
            {
                query = query.Where(p => p.IsPro == true);
            }
            else if (sorting == "isNew")
            {
                query = query.Where(p => p.IsNew == true);
            }
            else if (sorting == "isSale")
            {
                query = query.Where(p => p.IsSale == true);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.ProductPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.ProductPrice <= maxPrice.Value);
            }
            // Tính số sản phẩm của mỗi mức giá
            ViewData["AllPriceCount"] = query.Count();
            ViewData["Price1Count"] = query.Count(p => p.ProductPrice >= 0 && p.ProductPrice <= 1000000);
            ViewData["Price2Count"] = query.Count(p => p.ProductPrice > 1000000 && p.ProductPrice <= 10000000);
            ViewData["Price3Count"] = query.Count(p => p.ProductPrice > 10000000 && p.ProductPrice <= 50000000);


            page = page < 1 ? 1 : page;
            var products = query.ToPagedList(page, pageSize);
            if (products.PageNumber != 1 && page > products.PageCount)
            {
                page = products.PageCount;
                products = query.ToPagedList(page, pageSize);
            }
            ViewBag.TotalPages = products.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = products.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = products.TotalItemCount;
            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;

            ViewBag.SelectedCategoryName = categoryName;


            return View(products);
        }

        //public IActionResult Index(string? categoryName, string? filter, string sorting, int? minPrice, int? maxPrice, int page = 1, int pageSize = 6)
        //{
        //    var query = _context.Products
        //        .Include(p => p.Category)
        //        .Where(p => p.IsActive == true && (string.IsNullOrEmpty(filter) || p.Filter.Contains(filter!)));

        //    if (!string.IsNullOrEmpty(categoryName))
        //    {
        //        query = query.Where(p => p.Category.CategoryName == categoryName);
        //    }

        //    if (sorting == "isPro")
        //    {
        //        query = query.Where(p => p.IsPro == true);
        //    }
        //    else if (sorting == "isNew")
        //    {
        //        query = query.Where(p => p.IsNew == true);
        //    }
        //    else if (sorting == "isSale")
        //    {
        //        query = query.Where(p => p.IsSale == true);
        //    }

        //    if (minPrice.HasValue)
        //    {
        //        query = query.Where(p => p.ProductPrice >= minPrice.Value);
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        query = query.Where(p => p.ProductPrice <= maxPrice.Value);
        //    }

        //    // Tính số sản phẩm của mỗi mức giá
        //    ViewData["AllPriceCount"] = query.Count();
        //    ViewData["Price1Count"] = query.Count(p => p.ProductPrice >= 0 && p.ProductPrice <= 1000000);
        //    ViewData["Price2Count"] = query.Count(p => p.ProductPrice > 1000000 && p.ProductPrice <= 10000000);
        //    ViewData["Price3Count"] = query.Count(p => p.ProductPrice > 10000000 && p.ProductPrice <= 50000000);

        //    page = page < 1 ? 1 : page;
        //    var products = query.ToPagedList(page, pageSize);
        //    if (products.PageNumber != 1 && page > products.PageCount)
        //    {
        //        page = products.PageCount;
        //        products = query.ToPagedList(page, pageSize);
        //    }

        //    ViewBag.TotalPages = products.PageCount;
        //    ViewBag.CurrentPage = page;
        //    ViewBag.PageSize = pageSize;

        //    TempData["page"] = page;
        //    TempData["endpage"] = products.PageCount;
        //    TempData["PageSize"] = pageSize;

        //    ViewBag.TotalRows = products.TotalItemCount;

        //    int itemCount = (page - 1) * pageSize + 1;
        //    ViewBag.ItemCount = itemCount;

        //    return View(products);
        //}



    }
}
