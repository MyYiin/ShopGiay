using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;
using ShopGiay.Enums;

namespace ShopGiay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HoadonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoadonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ====================================================
        // 1. DANH SÁCH HÓA ĐƠN
        // ====================================================
        public async Task<IActionResult> Index(int? trangThai)
        {
            var query = _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .AsQueryable();

            // Lọc theo trạng thái enum
            if (trangThai.HasValue)
            {
                query = query.Where(h => h.TrangThai == (Status)trangThai.Value);
            }

            var orders = await query
                .OrderByDescending(h => h.Ngay)
                .ToListAsync();

            ViewBag.TrangThaiFilter = trangThai;
            return View(orders);
        }

        // ====================================================
        // 2. CHI TIẾT HÓA ĐƠN
        // ====================================================
        public async Task<IActionResult> Details(int id)
        {
            var hoadon = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                    .ThenInclude(kh => kh.Diachis)
                .Include(h => h.Cthoadons)
                    .ThenInclude(ct => ct.MaMhNavigation)
                .Include(h => h.Cthoadons)
                    .ThenInclude(ct => ct.MaMsNavigation)
                .Include(h => h.Cthoadons)
                    .ThenInclude(ct => ct.MaKcNavigation)
                .FirstOrDefaultAsync(h => h.MaHd == id);

            if (hoadon == null)
                return NotFound();

            return View(hoadon);
        }

        // ====================================================
        // 3. ĐỔI TRẠNG THÁI
        // ====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, int trangThai)
        {
            var hoadon = await _context.Hoadons
                .Include(h => h.Cthoadons)
                .FirstOrDefaultAsync(h => h.MaHd == id);

            if (hoadon == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Details), new { id });
            }

            var newStatus = (Status)trangThai;
            if (!IsValidStatusChange(hoadon.TrangThai, newStatus))
            {
                TempData["Error"] = "Không thể chuyển sang trạng thái này";
                return RedirectToAction(nameof(Details), new { id });
            }

            hoadon.TrangThai = newStatus;

            // Nếu chuyển sang Hoàn Thành -> tăng LuotMua cho các sản phẩm
            if (newStatus == Status.HoanThanh)
            {
                foreach (var ct in hoadon.Cthoadons)
                {
                    var mathang = await _context.Mathangs.FindAsync(ct.MaMh);
                    if (mathang != null)
                    {
                        mathang.LuotMua = (mathang.LuotMua ?? 0) + (ct.SoLuong ?? 0);
                        _context.Update(mathang);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ====================================================
        // 4. HỦY ĐƠN – hoàn trả tồn kho
        // ====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id, string reason)
        {
            var hoadon = await _context.Hoadons
                .Include(h => h.Cthoadons)
                .FirstOrDefaultAsync(h => h.MaHd == id);

            if (hoadon == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (hoadon.TrangThai == Status.HoanThanh)
            {
                TempData["Error"] = "Không thể hủy đơn hàng đã hoàn thành";
                return RedirectToAction(nameof(Details), new { id });
            }

            // hoàn kho
            foreach (var ct in hoadon.Cthoadons)
            {
                var ton = await _context.Tonkhos.FindAsync(ct.MaK);
                if (ton != null)
                {
                    ton.SoLuongTonKho += ct.SoLuong;
                    _context.Update(ton);
                }
            }

            hoadon.TrangThai = Status.DaHuy;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã hủy đơn hàng. Lý do: {reason}";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ====================================================
        // 5. QUY TẮC CHUYỂN TRẠNG THÁI
        // ====================================================
        private bool IsValidStatusChange(Status? currentStatus, Status newStatus)
        {
            switch (currentStatus)
            {
                case Status.ChoXuLy:
                    return newStatus == Status.DaXacNhan || newStatus == Status.DaHuy;

                case Status.DaXacNhan:
                    return newStatus == Status.DangGiaoHang || newStatus == Status.DaHuy;

                case Status.DangGiaoHang:
                    return newStatus == Status.HoanThanh;

                default:
                    return false;
            }
        }
    }
}
