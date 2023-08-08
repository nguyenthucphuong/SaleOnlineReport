using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace SaleOnline.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected void SetPagination(IPagedList pagedList, int page, int pageSize)
        {
            ViewBag.TotalPages = pagedList.PageCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            TempData["page"] = page;
            TempData["endpage"] = pagedList.PageCount;
            TempData["PageSize"] = pageSize;
            ViewBag.TotalRows = pagedList.TotalItemCount;
            int itemCount = (page - 1) * pageSize + 1;
            ViewBag.ItemCount = itemCount;
            ViewBag.TickId = TempData["TickId"];
        }
    }
}
