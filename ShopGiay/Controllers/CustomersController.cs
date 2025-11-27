using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopGiay.Data;
using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopGiay.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Khachhang> _passwordHasher;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

       

        public CustomersController(ApplicationDbContext context, IPasswordHasher<Khachhang> passwordHasher, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager )
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
            _userManager = userManager;
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
            ViewData["loaigiay"] = _context.Loaigiays.FirstOrDefault(d => d.MaLg == id).Ten;
            ViewData["thuonghieu"] = _context.Thuonghieus.FirstOrDefault(t => t.MaTh == id).Ten;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            GetData();
            var applicationDbContext = _context.Mathangs.Include(m => m.MaThNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //GetData();
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var mathang = await _context.Mathangs
            //    .Include(m => m.MaThNavigation)
            //    .FirstOrDefaultAsync(m => m.MaMh == id);
            //if (mathang == null)
            //{
            //    return NotFound();
            //}

            //return View(mathang);
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

        // Lập hóa đơn: lưu hóa đơn, lưu chi tiết hóa đơn 
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateBill(string email, string hoten, string dienthoai, string diachi)
        //{
        //    // Validation
        //    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(hoten) ||
        //        string.IsNullOrWhiteSpace(dienthoai) || string.IsNullOrWhiteSpace(diachi))
        //    {
        //        TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin!";
        //        return RedirectToAction(nameof(CheckOut));
        //    }

        //    var cart = GetCartItems();
        //    if (cart == null || cart.Count == 0)
        //    {
        //        TempData["ErrorMessage"] = "Giỏ hàng trống!";
        //        return RedirectToAction(nameof(ViewCart));
        //    }

        //    try
        //    {
        //        // Kiểm tra xem khách hàng đã tồn tại chưa
        //        var kh = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Email == email);

        //        // Nếu chưa tồn tại thì tạo mới
        //        if (kh == null)
        //        {
        //            kh = new Khachhang();
        //            kh.Email = email;
        //            kh.Ten = hoten;
        //            kh.DienThoai = dienthoai;
        //            _context.Add(kh);
        //            await _context.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            // Cập nhật thông tin nếu khách hàng đã tồn tại
        //            kh.Ten = hoten;
        //            kh.DienThoai = dienthoai;
        //            _context.Update(kh);
        //            await _context.SaveChangesAsync();
        //        }

        //        // Lưu địa chỉ khách hàng
        //        var dc = new Diachi();
        //        dc.MaKh = kh.MaKh;
        //        dc.DiaChi1 = diachi;
        //        dc.MacDinh = 1; // Địa chỉ mặc định
        //        _context.Add(dc);
        //        await _context.SaveChangesAsync();

        //        var hd = new Hoadon();
        //        hd.Ngay = DateTime.Now;
        //        hd.MaKh = kh.MaKh;
        //        hd.TrangThai = 1; // Đã thanh toán
        //        _context.Add(hd);
        //        await _context.SaveChangesAsync();

        //        // thêm chi tiết hóa đơn - Load lại MatHang từ database để đảm bảo dữ liệu chính xác
        //        int thanhtien = 0;
        //        int tongtien = 0;
        //        foreach (var i in cart)
        //        {
        //            // Load lại MatHang từ database
        //            var mathang = await _context.Mathangs.FindAsync(i.MatHang.MaMh);
        //            if (mathang == null)
        //            {
        //                continue; // Bỏ qua sản phẩm không tồn tại
        //            }

        //            var ct = new Cthoadon();
        //            ct.MaHd = hd.MaHd;
        //            ct.MaMh = mathang.MaMh;

        //            thanhtien = (mathang.GiaBan ?? 0) * i.SoLuong;
        //            tongtien += thanhtien;
        //            ct.DonGia = mathang.GiaBan;
        //            ct.SoLuong = (short)i.SoLuong;
        //            ct.ThanhTien = thanhtien;
        //            _context.Add(ct);
        //        }
        //        await _context.SaveChangesAsync();

        //        // cập nhật tổng tiền hóa đơn 
        //        hd.TongTien = tongtien;
        //        _context.Update(hd);
        //        await _context.SaveChangesAsync();

        //        // Load lại hóa đơn với đầy đủ thông tin để hiển thị
        //        var hoadon = await _context.Hoadons
        //            .Include(h => h.MaKhNavigation)
        //            .Include(h => h.Cthoadons)
        //                .ThenInclude(ct => ct.MaMhNavigation)
        //            .FirstOrDefaultAsync(h => h.MaHd == hd.MaHd);

        //        // xóa giỏ hàng 
        //        ClearCart();
        //        GetData(); // Cập nhật số lượng về 0 sau khi xóa giỏ hàng

        //        return View(hoadon);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo hóa đơn: " + ex.Message;
        //        return RedirectToAction(nameof(CheckOut));
        //    }
        //}
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
                // Đăng nhập thành công, thực hiện các hành động cần thiết 
                // Ví dụ: Ghi thông tin người dùng vào Session 
                HttpContext.Session.SetString("khachhang", kh.Email);
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
                // Lấy thông tin khách hàng từ database với chi tiết hóa đơn
                var khachhang = await _context.Khachhangs
                    .Include(k => k.Diachis)
                    .Include(k => k.Hoadons)
                        .ThenInclude(h => h.Cthoadons)
                        .ThenInclude(ct => ct.MaMhNavigation)
                    .FirstOrDefaultAsync(k => k.Email == customerEmail);

                ViewBag.khachhang = khachhang;
            }

            return View();
        }

        public IActionResult Signout()
        {
            HttpContext.Session.SetString("khachhang", "");
            GetData();
            return RedirectToAction("Index");
        }
    }
}
