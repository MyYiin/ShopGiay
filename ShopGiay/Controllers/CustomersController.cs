using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using ShopGiay.Data;
using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopGiay.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Khachhang> _passwordHasher;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomersController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IPasswordHasher<Khachhang> passwordHasher, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager )
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        void GetData()
        {
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0)
            {
                ViewData["SoLuong"] = 0;
            }
            else
            {
                ViewData["SoLuong"] = cart.Sum(item => item.SoLuong);
            }

            ViewBag.danhmucs = _context.Loaigiays.ToList();

        }
        
        public async Task<IActionResult> List(int id)
        {
            GetData();
            var applicationDbContext = _context.Mathangs.Where(m => m.MaTh == id).Include(m => m.MaThNavigation);
            var mathangs = await applicationDbContext.ToListAsync();

            ViewData["loaigiay"] = _context.Loaigiays.FirstOrDefault(d => d.MaLg == id)?.Ten;
            ViewData["thuonghieu"] = _context.Thuonghieus.FirstOrDefault(t => t.MaTh == id)?.Ten;

            // Tính điểm đánh giá trung bình cho từng sản phẩm
            var danhGiaDict = await _context.Danhgia
                .GroupBy(dg => dg.MaMh)
                .Select(g => new
                {
                    MaMh = g.Key,
                    DiemTrungBinh = g.Average(dg => (double)dg.Diem),
                    SoLuongDanhGia = g.Count()
                })
                .ToDictionaryAsync(x => x.MaMh, x => new { x.DiemTrungBinh, x.SoLuongDanhGia });

            ViewBag.DanhGia = danhGiaDict;

            return View(mathangs);
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            GetData();
            var applicationDbContext = _context.Mathangs.Include(m => m.MaThNavigation);
            var mathangs = await applicationDbContext.ToListAsync();

            // Tính điểm đánh giá trung bình cho từng sản phẩm
            var danhGiaDict = await _context.Danhgia
                .GroupBy(dg => dg.MaMh)
                .Select(g => new
                {
                    MaMh = g.Key,
                    DiemTrungBinh = g.Average(dg => (double)dg.Diem),
                    SoLuongDanhGia = g.Count()
                })
                .ToDictionaryAsync(x => x.MaMh, x => new { x.DiemTrungBinh, x.SoLuongDanhGia });

            ViewBag.DanhGia = danhGiaDict;

            return View(mathangs);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            GetData();
            var mathang = _context.Mathangs
                .Include(m => m.Tonkhos)
                    .ThenInclude(t => t.MaKcNavigation)
                .Include(m => m.Tonkhos)
                    .ThenInclude(t => t.MaMsNavigation)
                .Include(m => m.MaLgNavigation)
                .Include(m => m.MaThNavigation)
                .FirstOrDefault(m => m.MaMh == id);

            if (mathang == null)
            {
                return NotFound();
            }
            var danhSachDanhGia = await _context.Danhgia
                .Where(dg => dg.MaMh == id)
                // Phải Include MaKhNavigation để lấy tên khách hàng trong View
                .Include(dg => dg.MaKhNavigation) 
                .ToListAsync();

            // 2. Tính toán điểm trung bình
            double averageRating = 0.0;
            int reviewCount = danhSachDanhGia.Count;
            if (reviewCount > 0)
            {
                // Sử dụng thuộc tính Diem trong Model Danhgia (chú ý chữ hoa/thường)
                averageRating = Math.Round(danhSachDanhGia.Average(dg => (double)dg.Diem), 1); // Làm tròn 1 chữ số thập phân
            }

            // 3. Kiểm tra khách hàng hiện tại đã đánh giá chưa
            bool hasReviewed = false;
            // Lấy MaKH từ Claims. Giả định bạn lưu MaKH là một Claim khi đăng nhập.
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst("MaKH")?.Value;
            
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int maKh))
            {
                // Kiểm tra nếu MaKH đã tồn tại trong danh sách đánh giá cho sản phẩm này
                hasReviewed = danhSachDanhGia.Any(dg => dg.MaKh == maKh);
            }
            
            // 4. Lấy danh sách biến thể (Giả định có bảng BienThe và Model tương ứng)
            // Cần BienThe để hiển thị dropdown Màu/Size
            var danhSachBienThe = await _context.Tonkhos
                .Where(bt => bt.MaMh == id)
                .ToListAsync();

            // Gán dữ liệu vào ViewBag
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.HasReviewed = hasReviewed;
            ViewBag.DanhSachDanhGia = danhSachDanhGia;
            ViewBag.BienThe = danhSachBienThe;
            // Tăng lượt xem khi user xem chi tiết sản phẩm
            mathang.LuotXem = (mathang.LuotXem ?? 0) + 1;
            _context.Update(mathang);
            await _context.SaveChangesAsync();

            return View(mathang);

        }

        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string? jsoncart = session.GetString("shopcart");
            if (jsoncart != null)
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
                return cartItems ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        // Lưu danh sách CartItem trong giỏ hàng vào session
        void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string jsoncart = JsonConvert.SerializeObject(list, settings);
            session.SetString("shopcart", jsoncart);
        }

        // Xóa session giỏ hàng 
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove("shopcart");
        }

        // Thêm hàng vào giỏ từ trang Index (không chọn màu/size)
        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            var cart = GetCartItems();

            // Kiểm tra xem sản phẩm này đã có trong giỏ hàng chưa (bất kỳ màu/size nào)
            var existingItem = cart.FirstOrDefault(p => p.MatHang.MaMh == id);

            if (existingItem != null)
            {
                // Nếu đã có trong giỏ, tăng số lượng của item đó
                existingItem.SoLuong++;
                SaveCartSession(cart);
                TempData["SuccessMessage"] = "Đã tăng số lượng sản phẩm trong giỏ hàng!";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Nếu chưa có, lấy biến thể đầu tiên còn hàng
            var tonkho = await _context.Tonkhos
                .Include(t => t.MaMhNavigation)
                .Include(t => t.MaMsNavigation)
                .Include(t => t.MaKcNavigation)
                .FirstOrDefaultAsync(t => t.MaMh == id && t.SoLuongTonKho > 0);

            if (tonkho == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm hết hàng!";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Thêm mới vào giỏ
            cart.Add(new CartItem()
            {
                TonkhoId = tonkho.MaK,
                MatHang = tonkho.MaMhNavigation,
                MauSac = tonkho.MaMsNavigation.Ten,
                KichCo = tonkho.MaKcNavigation.GiaTriKc,
                MaMs = tonkho.MaMs,
                MaKc = tonkho.MaKc,
                SoLuong = 1
            });

            SaveCartSession(cart);
            GetData();

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Thêm hàng vào giỏ từ trang Details (có chọn màu/size)
        [HttpPost]
        public async Task<IActionResult> AddToCart(int tonkhoId, int maMs, int maKc, int quantity = 1)
        {
            // Lấy thông tin tồn kho
            var tonkho = await _context.Tonkhos
                .Include(t => t.MaMhNavigation)
                .Include(t => t.MaMsNavigation)
                .Include(t => t.MaKcNavigation)
                .FirstOrDefaultAsync(t => t.MaK == tonkhoId);

            if (tonkho == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra tồn kho
            if (tonkho.SoLuongTonKho < quantity)
            {
                TempData["ErrorMessage"] = "Sản phẩm không đủ số lượng trong kho!";
                return RedirectToAction("Details", new { id = tonkho.MaMh });
            }

            var cart = GetCartItems();

            // Tìm xem đã có sản phẩm này với màu và size này trong giỏ chưa
            var item = cart.Find(p => p.MatHang.MaMh == tonkho.MaMh && p.MaMs == maMs && p.MaKc == maKc);

            if (item != null)
            {
                item.SoLuong += quantity;
            }
            else
            {
                cart.Add(new CartItem()
                {
                    TonkhoId = tonkhoId,
                    MatHang = tonkho.MaMhNavigation,
                    MauSac = tonkho.MaMsNavigation.Ten,
                    KichCo = tonkho.MaKcNavigation.GiaTriKc,
                    MaMs = maMs,
                    MaKc = maKc,
                    SoLuong = quantity
                });
            }

            SaveCartSession(cart);
            GetData();

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Details", new { id = tonkho.MaMh });
        }

        // Chuyển đến view xem giỏ hàng 
        public IActionResult ViewCart()
        {
            GetData();
            return View(GetCartItems());
        }

        // Xóa một mặt hàng khỏi giỏ
        public IActionResult RemoveItem(int tonkhoId, int maMs, int maKc)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.TonkhoId == tonkhoId && p.MaMs == maMs && p.MaKc == maKc);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCartSession(cart);
            GetData();
            return RedirectToAction(nameof(ViewCart));
        }

        // Cập nhật số lượng một mặt hàng trong giỏ
        [HttpPost]
        public IActionResult UpdateItem(int tonkhoId, int maMs, int maKc, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.TonkhoId == tonkhoId && p.MaMs == maMs && p.MaKc == maKc);
            if (item != null)
            {
                item.SoLuong = quantity;
            }
            SaveCartSession(cart);
            GetData();
            return RedirectToAction(nameof(ViewCart));
        }

        // Hiển thị trang thanh toán
        public async Task<IActionResult> CheckOut()
        {
            GetData();
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction(nameof(ViewCart));
            }

            // Kiểm tra nếu khách hàng đã đăng nhập
            var customerEmail = HttpContext.Session.GetString("khachhang");
            if (!string.IsNullOrEmpty(customerEmail))
            {
                var khachhang = await _context.Khachhangs
                    .Include(k => k.Diachis)
                    .FirstOrDefaultAsync(k => k.Email == customerEmail);

                if (khachhang != null)
                {
                    // Truyền thông tin khách hàng sang view
                    ViewBag.CustomerEmail = khachhang.Email;
                    ViewBag.CustomerName = khachhang.Ten;
                    ViewBag.CustomerPhone = khachhang.DienThoai;

                    // Lấy địa chỉ mặc định nếu có
                    var defaultAddress = khachhang.Diachis?.FirstOrDefault(d => d.MacDinh == 1);
                    ViewBag.CustomerAddress = defaultAddress?.DiaChi1 ?? "";
                }
            }

            return View(cart);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBill(string email, string hoten, string dienthoai, string diachi)
        {
            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(hoten) ||
                string.IsNullOrWhiteSpace(dienthoai) || string.IsNullOrWhiteSpace(diachi))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin!";
                return RedirectToAction(nameof(CheckOut));
            }

            var cart = GetCartItems();
            if (cart == null || cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction(nameof(ViewCart));
            }

            try
            {
                // === KIỂM TRA KHÁCH HÀNG ===
                var kh = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);

                if (kh == null)
                {
                    kh = new Khachhang
                    {
                        Email = email,
                        Ten = hoten,
                        DienThoai = dienthoai
                    };
                    _context.Add(kh);
                }
                else
                {
                    kh.Ten = hoten;
                    kh.DienThoai = dienthoai;
                    _context.Update(kh);
                }
                await _context.SaveChangesAsync();


                // === LƯU ĐỊA CHỈ ===
                var dc = new Diachi
                {
                    MaKh = kh.MaKh,
                    DiaChi1 = diachi,
                    MacDinh = 1
                };
                _context.Add(dc);
                await _context.SaveChangesAsync();


                // === TẠO HOÁ ĐƠN ===
                var hd = new Hoadon
                {
                    MaKh = kh.MaKh,
                    Ngay = DateTime.Now,
                    TrangThai = 0  // Đổi từ 1 thành 0 (Chờ xử lý)
                };
                _context.Add(hd);
                await _context.SaveChangesAsync();


                // === TẠO CHI TIẾT HOÁ ĐƠN ===
                int tongtien = 0;

                foreach (var i in cart)
                {
                    // LẤY BIẾN THỂ ĐÚNG TỪ TỒN KHO
                    var tonkho = await _context.Tonkhos
                        .Include(t => t.MaMsNavigation)
                        .Include(t => t.MaKcNavigation)
                        .FirstOrDefaultAsync(t =>
                            t.MaMh == i.MatHang.MaMh &&
                            t.MaMs == i.MaMs &&
                            t.MaKc == i.MaKc);

                    if (tonkho == null)
                    {
                        TempData["ErrorMessage"] = "Sản phẩm không tồn tại trong kho!";
                        return RedirectToAction(nameof(ViewCart));
                    }

                    if (tonkho.SoLuongTonKho < i.SoLuong)
                    {
                        TempData["ErrorMessage"] =
                            $"Sản phẩm {i.MatHang.Ten} ({tonkho.MaMsNavigation.Ten}, size {tonkho.MaKcNavigation.GiaTriKc}) không đủ số lượng!";
                        return RedirectToAction(nameof(ViewCart));
                    }

                    // TÍNH THÀNH TIỀN
                    int dongia = tonkho.GiaBanBt ?? 0;
                    int thanhtien = dongia * i.SoLuong;
                    tongtien += thanhtien;

                    // LƯU CHI TIẾT HÓA ĐƠN
                    var ct = new Cthoadon
                    {
                        MaHd = hd.MaHd,
                        MaK = tonkho.MaK,
                        MaMh = tonkho.MaMh,
                        MaMs = tonkho.MaMs,
                        MaKc = tonkho.MaKc,
                        DonGia = dongia,
                        SoLuong = (short)i.SoLuong,
                        ThanhTien = thanhtien
                    };
                    _context.Add(ct);

                    // TRỪ TỒN KHO
                    tonkho.SoLuongTonKho -= (short)i.SoLuong;
                    _context.Update(tonkho);
                }

                await _context.SaveChangesAsync();


                // === CẬP NHẬT TỔNG TIỀN HOÁ ĐƠN ===
                hd.TongTien = tongtien;
                _context.Update(hd);
                await _context.SaveChangesAsync();


                // === LOAD ĐẦY ĐỦ DỮ LIỆU ĐỂ RETURN VIEW HOÁ ĐƠN ===
                var hoadon = await _context.Hoadons
                    .Include(h => h.MaKhNavigation)
                    .Include(h => h.Cthoadons)
                        .ThenInclude(ct => ct.MaMhNavigation)
                    .Include(h => h.Cthoadons)
                        .ThenInclude(ct => ct.MaMsNavigation)
                    .Include(h => h.Cthoadons)
                        .ThenInclude(ct => ct.MaKcNavigation)
                    .FirstOrDefaultAsync(h => h.MaHd == hd.MaHd);


                // XÓA GIỎ HÀNG
                ClearCart();
                GetData();

                return View("CreateBill", hoadon);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo hóa đơn: " + ex.Message;
                return RedirectToAction(nameof(CheckOut));
            }
        }


        public IActionResult Register()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string hoten, string dienthoai,string email, string matkhau)
        {
            Khachhang kh = new Khachhang();
            kh.Ten = hoten;
            kh.DienThoai = dienthoai;
            kh.Email = email;
            kh.MatKhau = _passwordHasher.HashPassword(kh, matkhau); // mã hóa mk    

            if (ModelState.IsValid)
            {
                _context.Add(kh);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Login));
        }
        
        public IActionResult Login()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string matkhau)
        {

            var admin = await _userManager.FindByEmailAsync(email);

            if (admin != null)
            {
                var result = await _signInManager.PasswordSignInAsync(admin, matkhau, false, false);

                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(admin, "Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }

            var kh = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.Email == email);
            if (kh != null && _passwordHasher.VerifyHashedPassword(kh, kh.MatKhau, matkhau) == PasswordVerificationResult.Success)
            {
                // Đăng nhập thành công
                // 1. Thiết lập Session
                HttpContext.Session.SetString("khachhang", kh.Email);

                // 2. Tạo Claims cho authentication
                var claims = new List<Claim>
                {
                    new Claim("MaKH", kh.MaKh.ToString()),
                    new Claim(ClaimTypes.Email, kh.Email),
                    new Claim(ClaimTypes.Name, kh.Ten),
                    new Claim(ClaimTypes.NameIdentifier, kh.MaKh.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Customer");
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true, // Cookie sẽ tồn tại sau khi đóng browser
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync("Customer", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction(nameof(CustomerInfo));

            }
            // Sai thông tin
            TempData["LoiDangNhap"] = "Email hoặc mật khẩu không đúng!";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> CustomerInfo()
        {
            GetData();

            // Lấy email khách hàng từ session
            var customerEmail = HttpContext.Session.GetString("khachhang");

            if (!string.IsNullOrEmpty(customerEmail))
            {
                // Lấy thông tin khách hàng từ database với chi tiết hóa đơn đầy đủ
                var khachhang = await _context.Khachhangs
                    .Include(k => k.Diachis)
                    .Include(k => k.Hoadons)
                        .ThenInclude(h => h.Cthoadons)
                            .ThenInclude(ct => ct.MaMhNavigation)
                    .Include(k => k.Hoadons)
                        .ThenInclude(h => h.Cthoadons)
                            .ThenInclude(ct => ct.MaMsNavigation)
                    .Include(k => k.Hoadons)
                        .ThenInclude(h => h.Cthoadons)
                            .ThenInclude(ct => ct.MaKcNavigation)
                    .FirstOrDefaultAsync(k => k.Email == customerEmail);

                ViewBag.khachhang = khachhang;
            }

            return View();
        }

        public async Task<IActionResult> Signout()
        {
            // Xóa session
            HttpContext.Session.SetString("khachhang", "");
            ClearCart(); // Xóa giỏ hàng khi đăng xuất

            // Sign out khỏi authentication scheme
            await HttpContext.SignOutAsync("Customer");

            return RedirectToAction("Index");
        }
        public IActionResult About()
        {
            return View();
        }
        // Thêm Action này vào CustomersController
        // POST: Customers/GuiDanhGia
        [HttpPost]
        [Authorize] // Yêu cầu khách hàng phải đăng nhập
        public async Task<IActionResult> GuiDanhGia(int MaMh, int Diem, string NoiDung)
        {
            // 1. Lấy MaKH của khách hàng đang đăng nhập (giả định đã lưu trong Claim "MaKH")
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst("MaKH")?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int maKh))
            {
                // Trả về lỗi nếu không tìm thấy MaKH hợp lệ (mặc dù đã có [Authorize])
                TempData["Error"] = "Vui lòng đăng nhập lại để thực hiện chức năng này.";
                return RedirectToAction(nameof(Details), new { id = MaMh });
            }

            // 2. Kiểm tra khách hàng đã đánh giá sản phẩm này chưa (đảm bảo UNIQUE (MaKH, MaMh))
            var existingReview = await _context.Danhgia
                .FirstOrDefaultAsync(dg => dg.MaMh == MaMh && dg.MaKh == maKh);

            if (existingReview != null)
            {
                TempData["Error"] = "Bạn đã đánh giá sản phẩm này trước đó rồi.";
                return RedirectToAction(nameof(Details), new { id = MaMh });
            }

            // 3. Tạo đối tượng đánh giá mới
            var newReview = new Danhgia
            {
                MaMh = MaMh,
                MaKh = maKh,
                Diem = Diem,
                NoiDung = NoiDung,
                NgayDg = DateTime.Now 
            };

            // 4. Lưu vào cơ sở dữ liệu
            try
            {
                _context.Add(newReview);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cảm ơn bạn đã gửi đánh giá thành công!";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi lưu DB (ví dụ: ràng buộc khóa ngoại, lỗi kết nối,...)
                TempData["Error"] = "Có lỗi xảy ra khi lưu đánh giá: " + ex.Message;
            }

            // Quay lại trang chi tiết sản phẩm
            return RedirectToAction(nameof(Details), new { id = MaMh });
        }


        [HttpGet]
        [Authorize] // Đảm bảo chỉ khách hàng đã đăng nhập mới vào được trang này
        public IActionResult ReviewProduct(int id) // id chính là MaMh
        {
            // 1. Tìm sản phẩm theo id (MaMh)
            var matHang = _context.Mathangs.Find(id);

            // 2. Tùy chọn: Kiểm tra xem người dùng có quyền đánh giá sản phẩm này (đã mua) không.
            //Đây là bước quan trọng để tránh lỗi logic trước đây.
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var daMua = _context.Cthoadons.Any(ct =>
                ct.MaMh == id && ct.MaHdNavigation.MaKhNavigation.Ten== userId);

            if (!daMua) return RedirectToAction("CustomerInfo", new { Message = "Bạn chưa mua sản phẩm này.", Type = "danger" });

            return View(matHang);
        }
        [HttpPost]
        [Authorize]
        public IActionResult SubmitReview(Danhgia model)
        {
            // 1. Lấy MaKh đang đăng nhập
            var maKh = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // 2. Gán các trường còn thiếu
            model.NgayDg = DateTime.Now;

            // 3. Lưu vào DB
            _context.Danhgia.Add(model);
            _context.SaveChanges();

            // 4. Chuyển hướng về trang chi tiết sản phẩm
            return RedirectToAction("Details", new { id = model.MaMh });
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Khachhangs.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                TempData["Error"] = "Email không tồn tại!";
                return View();
            }

            return RedirectToAction("ResetPassword", new { email = email });
        }

        // Trang nhập mật khẩu mới
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        // -----------------------------
        // 2) ĐỔI MẬT KHẨU
        // -----------------------------
        [HttpPost]
        public IActionResult ResetPassword(string email, string newPassword)
        {
            var user = _context.Khachhangs.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản!";
                return View();
            }

            // Hash mật khẩu bằng PasswordHasher
            user.MatKhau = _passwordHasher.HashPassword(user, newPassword);

            _context.SaveChanges();

            TempData["Success"] = "Đặt lại mật khẩu thành công! Hãy đăng nhập lại.";

            return RedirectToAction("Login");
        }


    }
}
