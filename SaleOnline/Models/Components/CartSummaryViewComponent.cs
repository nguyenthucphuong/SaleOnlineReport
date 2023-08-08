using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SaleOnline.Models.Components
{
    
    public class CartSummaryViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            int totalQuantity = 0;
            if (Request.Cookies.TryGetValue("Cart", out var cartJson))
            {
                var cartCookie = JsonConvert.DeserializeObject<List<CartCookie>>(cartJson) ?? new List<CartCookie>();
                totalQuantity = cartCookie.Sum(c => c.Quantity);
            }
            return Task.FromResult((IViewComponentResult)View(totalQuantity));
        }
    }

}
