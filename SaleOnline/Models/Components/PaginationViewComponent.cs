using Microsoft.AspNetCore.Mvc;

namespace SaleOnline.Models.Components
{
    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int currentPage, int totalPages, int pageSize, string search, string actionName)
        {
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.ActionName = actionName;
            return View();
        }
    }
}
