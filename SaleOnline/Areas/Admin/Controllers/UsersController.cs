using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SaleOnline.Models;
using X.PagedList;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace SaleOnline.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("taikhoan")]
    public class UsersController : BaseController
    {
        private readonly SaleOnline1Context _context;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(SaleOnline1Context context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.roleManager = roleManager;
        }

        [Route("danhdach")]
        public IActionResult Index(string? search, int page = 1, int pageSize = 5)
        {
            var query = _context.Users.Where(c => string.IsNullOrEmpty(search) || c.Filter.Contains(search!));
            page = page < 1 ? 1 : page;
            var users = query.ToPagedList(page, pageSize);
            if (users.PageNumber != 1 && page > users.PageCount)
            {
                page = users.PageCount;
                users = query.ToPagedList(page, pageSize);
            }
            
            SetPagination(users, page, pageSize);
            return View(users);
        }
        [HttpPost]
        public IActionResult StatusUpdate(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(k => k.UserId == userId);

            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _context.Users.Update(user);
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái thành công!";
            }
            int page = (int)TempData["page"]!;
            int pageSize = (int)TempData["PageSize"]!;

            TempData["TickId"] = user?.UserId;
            return RedirectToAction("Index", new { page, pageSize });
        }
        [Route("xoa")]
        public IActionResult Xoa(Guid userId)
        {
            var item = _context.Users.FirstOrDefault(k => k.UserId == userId);

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
                    StatusUpdate(userId);
                    TempData["Message"] = "Tài khoản đã được Tạm khóa trước khi xóa!";
                }
            }
            return RedirectToAction("Index", new { page, pageSize });
        }


        [HttpPost]
        [Route("them")]
        public IActionResult Them(string userName, string password, string email, Guid roleId, string? kichHoat)
        {
            var roles = _context.Roles.Where(r => r.IsActive == true).ToList();
            ViewBag.Roles = roles;
            ViewBag.RoleIds = roles.ToDictionary(r => r.RoleName, r => r.RoleId);
            if (!string.IsNullOrEmpty(userName))
            {
                SaleOnline.Models.User user = new SaleOnline.Models.User(
                    userName.ToLower(),
                    BCrypt.Net.BCrypt.HashPassword(password),
                email.ToLower(),
                    roleId,
                    kichHoat == "on" ? true : false);
                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["TickId"] = user.UserId;
                TempData["Message"] = "Thêm tài khoản mới thành công!";
                int pageSize = (int)TempData["PageSize"]!;
                var allUsers = _context.Users.OrderBy(c => c.UserId).ToList();
                var newUserIndex = allUsers.FindIndex(c => c.UserId == user.UserId);
                var newUserPage = (int)Math.Ceiling((newUserIndex + 1) / (double)pageSize);
                return RedirectToAction("Index", new { page = newUserPage, pageSize });
            }
            else
            {
                SaleOnline.Models.User user = new SaleOnline.Models.User();
                return View(user);
            }
        }


        [Route("capnhat")]
        public IActionResult CapNhat(Guid userId)
        {
            var roles = _context.Roles.Where(r => r.IsActive == true).ToList();
            ViewBag.Roles = roles;
            ViewBag.RoleIds = roles.ToDictionary(r => r.RoleName, r => r.RoleId);
            var user = _context.Users.FirstOrDefault(r => r.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }
            var selectedRole = roles.FirstOrDefault(u => u.RoleId == user.RoleId);
            TempData["SelectedRoleName"] = selectedRole?.RoleName;

            return View(user);
        }

        [HttpPost]
        [Route("capnhat")]
        public IActionResult CapNhat(Guid? userId, string userName, string password, string email, Guid roleId, string? kichHoat)
        {
            if (!userId.HasValue)
            {
                return RedirectToAction("Index");
            }
            var user = _context.Users.FirstOrDefault(k => k.UserId == userId.Value);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    user.UserName = userName.ToLower();
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    user.Email = email.ToLower();
                    user.RoleId = roleId;
                    user.Filter = userName.ToLower() + " " + email.ToLower();
                    user.IsActive = kichHoat == "on";
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    TempData["Message"] = "Cập nhật Tài khoản thành công!";
                    int page = (int)TempData["page"]!;
                    int pageSize = (int)TempData["PageSize"]!;
                    TempData["TickId"] = user.UserId;
                    return RedirectToAction("Index", new { page, pageSize });
                }
                else
                {
                    return View(user);
                }
            }
            return RedirectToAction("Index");
        }

        [Route("checkusermail")]
        public IActionResult CheckUserEmail(string username, string email)
        {
            var existingUser = _context.Users.FirstOrDefault(u => (username != null && u.UserName.ToLower() == username.ToLower()) || (email != null && u.Email.ToLower() == email.ToLower()));
            if (existingUser != null)
            {
                if (username != null && existingUser.UserName.ToLower() == username.ToLower())
                {
                    return Json(new { exists = true, type = "username" });
                }
                else
                {
                    return Json(new { exists = true, type = "email" });
                }
            }
            else
            {
                return Json(new { exists = false });
            }
        }

        [Route("dangki")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [Route("dangki")]
        public IActionResult Register(string username, string password, string email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Usename, Password, email không hợp lệ!";
                return BadRequest();

            }

            var customerRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Customer");
            if (customerRole == null)
            {
                TempData["Message"] = "Đang tạm dừng đăng kí tài khoản!";
                return BadRequest();
            }

            var user = new SaleOnline.Models.User
            {
                UserName = username.ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email.ToLower(),
                RoleId = customerRole.RoleId,
                Filter = username.ToLower() + " " + email.ToLower()
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            TempData["Message"] = "Đăng kí thành công!";
            return RedirectToAction("Login", "Users", new { area = "Admin" });

        }

        [Route("dangnhap")]
        public IActionResult Login()
        {
            return View();
        }
       
        [HttpPost]
        [Route("dangnhap")]
        public ActionResult Login(string email, string password, bool rememberMe)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
 
                Response.Cookies.Append("UserId", user.UserId.ToString());
                Response.Cookies.Append("UserName", user.UserName);
                Response.Cookies.Append("RoleId", user.RoleId.ToString());
                var role = _context.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                if (role != null)
                {
                    Response.Cookies.Append("RoleName", role.RoleName);
                    if (role.RoleName == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (role.RoleName == "User")
                    {
                         return RedirectToAction("Index", "Home", new { area = "User" });

                    }
                    else if (role.RoleName == "Customer")
                    {
                        return RedirectToAction("Index", "Shops");
                    }
                }
            }
            else
            {
                TempData["Message"] = "Email hoặc mật khẩu không đúng";
            }
            var model = new SaleOnline.Models.User { Email = email, Password = password };
            return View(model);
        }

        [Route("quenmatkhau")]
        public IActionResult ForgetPassword(string email)
        {
            // Kiểm tra email có null hoặc rỗng không
            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Vui lòng nhập lại đúng Email mà bạn đã đăng kí khi tạo tài khoản!";
                return RedirectToAction("Login", "Users");
            }
            // Kiểm tra email có tồn tại trong dữ liệu hay không
            var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (existingUser == null)
            {
                TempData["Message"] = "Vui lòng nhập lại đúng Email mà bạn đã đăng kí khi tạo tài khoản!";
                return RedirectToAction("Login", "Users");
            }
            ViewBag.Email = email;  
            return View();
        }



        [HttpPost]
        [Route("quenmatkhau")]
        public ActionResult ForgetPassword(string email, string password, string repeatPassword)
        {
            if (password == repeatPassword)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
                if (user != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    _context.SaveChanges();
                    TempData["Message"] = "Đổi mật khẩu thành công";
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                TempData["Message"] = "Mật khẩu nhập lại không khớp";
            }
            return View();
        }


        [Route("dangxuat")]
        public IActionResult Logout()
        {
            // Xóa thông tin người dùng khỏi cookies
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("RoleId");

            // Xóa giỏ hàng và thông tin đăng nhập khỏi cookies
            Response.Cookies.Delete("Cart");
            Response.Cookies.Delete("LoginInfo");

            // Chuyển hướng người dùng đến trang đăng nhập
            return RedirectToAction("Login", "Users", new { area = "Admin" });
        }

    }
}
