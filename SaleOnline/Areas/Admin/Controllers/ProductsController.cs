using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using X.PagedList;
using Common.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace SaleOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("sanpham")]
    public class ProductsController : Controller
    {
        private readonly SaleOnline1Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(SaleOnline1Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }



        [Route("danhdach")]
        public IActionResult Index(string? filter, int page = 1, int pageSize = 5)
        {
            var query = _context.Products.Where(c => string.IsNullOrEmpty(filter) || c.Filter.Contains(filter!));
            page = page < 1 ? 1 : page;
            // Lấy danh sách sản phẩm và phân trang theo giá trị của biến page và pageSize
            var products = query.ToPagedList(page, pageSize);
            // Kiểm tra xem số trang hiện tại có lớn hơn tổng số trang hay không
            if (products.PageNumber != 1 && page > products.PageCount)
            {
                // Nếu có, gán giá trị của biến page bằng tổng số trang
                page = products.PageCount;
                // Lấy lại danh sách sản phẩm và phân trang theo giá trị mới của biến page và pageSize
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

            ViewBag.TickId = TempData["TickId"];
            return View(products);
        }

        [HttpPost]
        [Route("status")]
        public IActionResult StatusUpdate(Guid productId)
        {
            var product = _context.Products.FirstOrDefault(k => k.ProductId == productId);

            if (product != null)
            {
                product.IsActive = !product.IsActive;
                _context.Products.Update(product);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;

            TempData["TickId"] = product?.ProductId;
            return RedirectToAction("Index", new { page, pageSize });
        }
 
        [Route("xoa")]
        public IActionResult Xoa(Guid productId)
        {
            var item = _context.Products.FirstOrDefault(k => k.ProductId == productId);

            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;
            if (item != null)
            {
                if (item.IsActive == false)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    TempData["Message"] = "Bạn đã xóa thành công!";
                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    //item.IsActive = false;
                    //_context.Categories.Update(item);
                    //_context.SaveChanges();
                    StatusUpdate(productId);
                    TempData["Message"] = "Danh mục đã được Tạm khóa trước khi xóa!";
                }
            }
            return RedirectToAction("Index", new { page, pageSize });
        }

       

        private string UploadImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Định nghĩa một mảng các định dạng tệp hình ảnh được cho phép
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                // Lấy phần mở rộng của tệp hình ảnh và chuyển thành chữ thường
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                // Kiểm tra xem phần mở rộng của tệp hình ảnh có nằm trong danh sách các định dạng được cho phép hay không
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Message"] = "Định dạng tệp hình ảnh không hợp lệ!";
                    return null!;
                }
                else
                {
                    // Nếu có, lấy tên tệp hình ảnh
                    var fileName = Path.GetFileName(imageFile.FileName);
                    // Tạo đường dẫn lưu trữ tệp hình ảnh
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "template", "nalika", "img", "product", fileName);
                    // Mở một luồng ghi tệp và ghi nội dung của đối tượng imageFile vào tệp theo đường dẫn đã định nghĩa
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    // Trả về đường dẫn của tệp hình ảnh đã được lưu trữ
                    return "/template/nalika/img/product/" + fileName;
                }
            }
            // Nếu đối tượng imageFile không tồn tại hoặc kích thước bằng 0, trả về giá trị null
            return null!;
        }

        [HttpPost]
        [Route("them")]
        public IActionResult Them(Guid? userId, Guid? categoryId, Guid? promotionId, string productName, decimal productPrice, string? productDes, IFormFile imageFile, string? isNew, string? isSale, string? isPro, string filter, string? kichHoat)
        {
            var categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            var promotions = _context.Promotions.Where(p => p.IsActive == true).ToList();
            //var users = _context.Users.Where(u => u.IsActive == true).ToList();
            ViewBag.Categories = categories;
            ViewBag.Promotions = promotions;
            //ViewBag.Users = users;

            // chuyển đổi danh sách categories thành một từ điển, với khóa là giá trị của thuộc tính CategoryName, còn giá trị là giá trị của thuộc tính CategoryId
            ViewBag.CategoryIds = categories.ToDictionary(c => c.CategoryName, c => c.CategoryId);
            // ViewBag.CategoryIds: 
            // {
            //     "Category 1": 1,
            //     "Category 2": 2,
            //     "Category 3": 3
            // }

            var promotionIds = promotions.Where(p => p.Discount.HasValue)
                             //Nhóm các đối tượng đã lọc theo giá trị của thuộc tính Discount
                             .GroupBy(p => p.Discount!.Value)
                             //Chuyển đổi kết quả nhóm thành một từ điển, với khóa là giá trị của thuộc tính Discount (được chuyển thành chuỗi), còn giá trị là danh sách các PromotionId tương ứng
                             .ToDictionary(g => g.Key.ToString(), g => g.Select(p => p.PromotionId).ToList());
            ViewBag.PromotionIds = promotionIds;

            //ViewBag.UserIds = users.ToDictionary(u => u.UserName, u => u.UserId);

            //var user = _context.Users.FirstOrDefault(u => u.IsActive == true && u.Role.RoleName == "Admin");
            //ViewBag.User = user;

            var users = _context.Users.Where(u => u.IsActive == true && u.Role.RoleName == "Admin").ToList();
            ViewBag.Users = (List<SaleOnline.Models.User>)users;

            if (!string.IsNullOrEmpty(productName))
            {
                Product product = new Product(
                    userId,
                    categoryId,
                    promotionId,
                    productName,
                    productPrice,
                    productDes,
                    null,
                    isNew == "on" ? true : false,
                    isSale == "on" ? true : false,
                    isPro == "on" ? true : false,
                    filter,
                    kichHoat == "on" ? true : false);
               
                _context.Products.Add(product);
                  _context.SaveChanges();

                TempData["TickId"] = product.ProductId;

                var filePath = UploadImage(imageFile);
                if (filePath != null)
                {
                    product.ProductImage = filePath;
                    _context.SaveChanges();
                }
                
                TempData["Message"] = "Thêm sản phẩm mới thành công!";
                int pageSize = (int)TempData["PageSize"]!;
                // Lấy danh sách tất cả sản phẩm và sắp xếp theo ProductId
                var allProducts = _context.Products.OrderBy(c => c.ProductId).ToList();
                // Tìm vị trí của sản phẩm mới trong danh sách sản phẩm,  được tính từ 0
                var newProductIndex = allProducts.FindIndex(c => c.ProductId == product.ProductId);
                // Tính số trang chứa sản phẩm mới
                //newProductIndex + 1: khi tính toán số trang chứa sản phẩm mới, chúng ta cần tính theo thứ tự của sản phẩm trong danh sách, bắt đầu từ 1. Do đó, chúng ta cần cộng thêm 1 vào newProductIndex để tính toán đúng số trang chứa sản phẩm mới.
                // Math.Ceiling((newProductIndex + 1) / (double)pageSize) làm tròn lên số nguyên gần nhất của số trang chứa sản phẩm mới.
                var newProductPage = (int)Math.Ceiling((newProductIndex + 1) / (double)pageSize);
                //Ví dụ: Giả sử newProductIndex = 9 và pageSize = 5, thì biểu thức sẽ tính toán như sau:
                //(newProductIndex + 1) / (double)pageSize = (9 + 1) / (double)5 = 2
                //Math.Ceiling((newProductIndex + 1) / (double)pageSize) = Math.Ceiling(2) = 2
                //(int)Math.Ceiling((newProductIndex + 1) / (double)pageSize) = (int)2 = 2
                //Vậy kết quả cuối cùng là 2, tức là sản phẩm mới nằm ở trang thứ hai trong danh sách sản phẩm được phân trang.
                return RedirectToAction("Index", new { page = newProductPage, pageSize });
            }
            else
            {
                Product product = new Product();
                return View(product);
            }
        }


        [Route("capnhat")]
        public IActionResult CapNhat(Guid productId)
        {
            //var categories = _context.Categories.ToList();
            //var promotions = _context.Promotions.ToList();
            var categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            var promotions = _context.Promotions.Where(p => p.IsActive == true).ToList();
            //var users = _context.Users.Where(u => u.IsActive == true).ToList();
            ViewBag.Categories = categories;
            ViewBag.Promotions = promotions;
            //ViewBag.Users = users;
            ViewBag.CategoryIds = categories.ToDictionary(c => c.CategoryName, c => c.CategoryId);
            //ViewBag.PromotionIds = promotions.Where(p => p.Discount.HasValue).ToDictionary(p => p.Discount!.Value.ToString(), p => p.PromotionId);

            var promotionIds = promotions.Where(p => p.Discount.HasValue)
                             .GroupBy(p => p.Discount!.Value)
                             .ToDictionary(g => g.Key.ToString(), g => g.Select(p => p.PromotionId).ToList());
            ViewBag.PromotionIds = promotionIds;

            //ViewBag.UserIds = users.ToDictionary(u => u.UserName, u => u.UserId);
            var users = _context.Users.Where(u => u.IsActive == true && u.Role.RoleName == "Admin").ToList();
            ViewBag.Users = (List<SaleOnline.Models.User>)users;


            var product = _context.Products.FirstOrDefault(k => k.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            var selectedUser = users.FirstOrDefault(u => u.UserId == product.UserId);
            TempData["SelectedUserName"] = selectedUser?.UserName;

            var selectedCategory = categories.FirstOrDefault(u => u.CategoryId == product.CategoryId);
            TempData["SelectedCategoryName"] = selectedCategory?.CategoryName;

            var selectedPromotion = promotions.FirstOrDefault(u => u.PromotionId == product.PromotionId);
            TempData["SelectedPromotionName"] = selectedPromotion?.Discount;

            return View(product);
        }

        [HttpPost]
        [Route("capnhat")]
        public IActionResult CapNhat(Guid? productId, Guid? userId, Guid? categoryId, Guid? promotionId, string productName, decimal? productPrice, string? productDes, IFormFile imageFile, string? isNew, string? isSale, string? isPro, string? filter, string? kichHoat)
        {
            if (!productId.HasValue)
            {
                return RedirectToAction("Index");
            }
            var product = _context.Products.FirstOrDefault(k => k.ProductId == productId.Value);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(productName))
                {
                    product.UserId = userId;
                    product.CategoryId = categoryId;
                    product.PromotionId = promotionId;
                    product.ProductName = productName.Trim();
                    product.ProductPrice = productPrice;
                    product.ProductDes = productDes?.Trim();
                    product.IsNew = isNew == "on";
                    product.IsSale = isSale == "on";
                    product.IsPro = isPro == "on";
                    product.Filter = product.ProductName.ToLower() + " " + product.ProductDes?.ToLower() + " " + Utility.ConvertToUnsign(product.ProductName.ToLower()) + " " + product.ProductName.ToLower() + " " + product.ProductId + " " + product.ProductDes?.ToLower() + " " + Utility.ConvertToUnsign(product.ProductDes?.ToLower() ?? string.Empty);
                    product.IsActive = kichHoat == "on";

                    var filePath = UploadImage(imageFile);
                    if (filePath != null)
                    {
                        product.ProductImage = filePath;
                    }
                    _context.Products.Update(product);
                    _context.SaveChanges();
                    TempData["Message"] = "Cập nhật sản phẩm thành công!";
                    int page = (int)TempData["page"]!;
                    int pageSize = (int)TempData["PageSize"]!;

                    //TempData["TickProductId"] = product.ProductId;
                    TempData["TickId"] = product.ProductId;
                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    return View(product);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
