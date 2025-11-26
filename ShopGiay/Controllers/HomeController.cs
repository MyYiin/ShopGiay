using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;
using System.Diagnostics;

namespace ShopGiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // Trang ch? qu?n tr? - Dashboard
        public async Task<IActionResult> Index()
        {
            // Th?ng kê t?ng quan
            ViewBag.TongSanPham = await _context.Mathangs.CountAsync();
            ViewBag.TongLoaiGiay = await _context.Loaigiays.CountAsync();
            ViewBag.TongThuongHieu = await _context.Thuonghieus.CountAsync();
            ViewBag.TongKhachHang = await _context.Khachhangs.CountAsync();
            ViewBag.TongHoaDon = await _context.Hoadons.CountAsync();
            ViewBag.TongDoanhThu = await _context.Hoadons.SumAsync(h => h.TongTien ?? 0);

            // Danh sách don  hàng gan day
            var hoaDonGanDay = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .OrderByDescending(h => h.Ngay)
                .Take(10)
                .ToListAsync();

            return View(hoaDonGanDay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignoutAdmin()
        {
            // 1. G?i ph??ng th?c SignOutAsync() c?a Identity ?? xóa cookie ??ng nh?p
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Customers", new { area = "" });
        }
    }
}
