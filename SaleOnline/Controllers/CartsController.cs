using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SaleOnline.Models;
using Common.Utilities;
using Newtonsoft.Json;
using System.Security.Claims;

namespace SaleOnline.Controllers
{
    public class CartsController : Controller
    {
        private readonly SaleOnline1Context _context;
        public CartsController(SaleOnline1Context context)
        {
            _context = context;
        }
        private void SaveCartToCookies(List<Cart> cart)
        {
            var cartCookie = cart.Select(c => new CartCookie
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UserId = c.UserId
            }).ToList();
            var cartJson = JsonConvert.SerializeObject(cartCookie);
            Response.Cookies.Append("Cart", cartJson);
        }
        private List<Cart> GetCartFromCookies()
        {
            if (Request.Cookies.TryGetValue("Cart", out var cartJson))
            {
                // Sử dụng phương thức JsonConvert.DeserializeObject để chuyển đổi một chuỗi JSON thành một danh sách các đối tượng CartCookie.
                //  Nếu chuỗi JSON không hợp lệ hoặc không thể chuyển đổi thành danh sách CartCookie, phép toán ?? sẽ trả về một danh sách CartCookie mới.
                //  Điều này đảm bảo rằng biến cartCookie luôn có giá trị và không bị null.
                var cartCookie = JsonConvert.DeserializeObject<List<CartCookie>>(cartJson) ?? new List<CartCookie>();
                var cart = new List<Cart>();
                foreach (var item in cartCookie)
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product != null)
                    {
                        var cartItem = new Cart
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UserId = item.UserId,
                            ProductName = product.ProductName,
                            ProductImage = product.ProductImage,
                            ProductPrice = product.ProductPrice ?? 0,
                            Product = product
                        };
                        cart.Add(cartItem);
                    }
                }
                return cart;
            }
            return new List<Cart>();
        }

        public IActionResult Index()
        {
            var cart = GetCartFromCookies();
            var total = cart.Sum(c => c.Total);
            var totalDiscount = cart.Sum(c => c.TotalDiscount);
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(Guid productId, int quantity)
        {
            const int maxRows = 3;
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }
            var cart = GetCartFromCookies();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem == null)
            {
                if (cart.Count < maxRows)
                {
                    cartItem = new Cart
                    {
                        ProductId = productId,
                        ProductName = product.ProductName,
                        ProductImage = product.ProductImage,
                        ProductPrice = product.ProductPrice ?? 0,
                        Quantity = quantity
                    };
                    cart.Add(cartItem);
                }
                else
                {
                    TempData["ErrorMessage"] = $"Giỏ hàng chỉ được phép tối đa {maxRows} loại sản phẩm khác nhau. Bạn vui lòng xóa bớt hoặc thanh toán!";
                }
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            SaveCartToCookies(cart);
            return RedirectToAction("Index", "Shops");
        }

        public IActionResult Delete(Guid? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }
            var cart = GetCartFromCookies();
            var cartItem = cart.FirstOrDefault(k => k.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCartToCookies(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(Guid productId, int changeQuantity)
        {
            var cart = GetCartFromCookies();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity += changeQuantity;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }
            SaveCartToCookies(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCart(List<Cart> cartItems, string submitButton)
        {
            if (submitButton == "minus")
            {
                foreach (var item in cartItems)
                {
                    if (item.Quantity > 0)
                    {
                        item.Quantity--;
                    }
                }
            }
            else if (submitButton == "plus")
            {
                foreach (var item in cartItems)
                {
                    item.Quantity++;
                }
            }
            // long subTotal = 0; using System.Numerics; BigInteger subTotal = 0;
            decimal subTotal = 0;
            foreach (var item in cartItems)
            {
                subTotal += item.ProductPrice * item.Quantity;
            }
            ViewBag.SubTotal = subTotal;
            SaveCartToCookies(cartItems);
            return View("Index", cartItems);
        }

        public IActionResult Checkout()
        {
            var cart = GetCartFromCookies();

            if (!Request.Cookies.TryGetValue("UserId", out var userIdString))
            {
                return Redirect("/taikhoan/dangnhap");
            }
            if (Guid.TryParse(userIdString, out var userId))
            {
                var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);
                if (customer != null)
                {
                    ViewData["FullName"] = customer.FullName;
                    ViewData["Address"] = customer.Address;
                    ViewData["PhoneNumber"] = customer.PhoneNumber;
                }
            }
            else
            {
                return Redirect("/taikhoan/dangnhap");
            }
            return View(cart);
        }

        private void UpdateOrCreateCustomer(string fullName, string address, string phoneNumber, Guid userId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);
            if (customer != null)
            {
                customer.FullName = fullName.Trim();
                customer.Address = address.Trim();
                customer.PhoneNumber = phoneNumber;
                customer.Filter = customer.FullName + " " + customer.Address + " " + Utility.ConvertToUnsign(customer.FullName.ToLower()) + " " + Utility.ConvertToUnsign(customer.Address.ToLower() ?? "") + " " + phoneNumber;
            }
            else
            {
                customer = new Customer(fullName, address, phoneNumber, userId, true);
                _context.Customers.Add(customer);
            }
            _context.SaveChanges();
        }

        public IActionResult PlaceOrder(string FullName, string Address, string PhoneNumber, string payment)
        {
            var cart = GetCartFromCookies();
            if (cart.Count == 0)
            {
                return BadRequest("Không có sản phẩm nào trong giỏ hàng.");
            }
            // Kiểm tra xem cookies có chứa giá trị với tên là "UserId" hay không
            if (!Request.Cookies.TryGetValue("UserId", out var userIdString))
            {
                return Redirect("/taikhoan/dangnhap");
            }
            // Chuyển đổi chuỗi userIdString thành kiểu Guid
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Dữ liệu người dùng không hợp lệ.");
            }

            var payMent = _context.Payments.FirstOrDefault(p => p.PaymentName == payment);
            if (payMent == null)
            {
                return BadRequest("Không có dữ liệu thanh toán.");
            }
            var paymentId = payMent.PaymentId;

            // Tìm kiếm khuyến mãi đầu tiên trong database
            var promotion = _context.Promotions.FirstOrDefault();
            if (promotion == null)
            {
                return BadRequest("Không có dữ liệu khuyến mãi.");
            }
             var promotionId = promotion.PromotionId;

            // Tìm kiếm trạng thái đơn hàng có tên là "Đang xử lý"
            var orderStatus = _context.OrderStatuses.FirstOrDefault(os => os.OrderStatusName == "Đang xử lý");
            if (orderStatus == null)
            {
                return BadRequest("Không có dữ liệu trạng thái đơn hàng.");
            }
            // Lấy OrderStatusId của trạng thái đơn hàng
            var orderStatusId = orderStatus.OrderStatusId;

            UpdateOrCreateCustomer(FullName, Address, PhoneNumber, userId);

            var order = new Order
            {
                UserId = userId,
                PaymentId = paymentId,
                PromotionId = promotionId,
                OrderStatusId = orderStatusId,
                Total = cart.Sum(item => item.Total),
                OrderDatetime = DateTime.Now,
                IsActive = true,
                Filter = userId + " " + paymentId + " " + orderStatusId,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.ProductPrice,
                    OrderItemDatetime = DateTime.Now,
                    IsActive = true,
                    Filter = DateTime.Now.ToString()
                };
                order.OrderItems.Add(orderItem);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            Response.Cookies.Delete("Cart");
            TempData["Message"] = "Bạn đã đặt đơn hàng thành công!";
            return RedirectToAction("Index", "Shops");
        }

    }
}
