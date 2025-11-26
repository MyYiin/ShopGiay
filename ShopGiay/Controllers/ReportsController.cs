using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Enums;

namespace ShopGiay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports/Revenue
        public async Task<IActionResult> Revenue(DateTime? startDate, DateTime? endDate, string period = "day")
        {
            // Mặc định lấy dữ liệu 7 ngày gần nhất
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddDays(-7);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            // Lấy các hóa đơn đã hoàn thành trong khoảng thời gian
            var orders = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.Cthoadons)
                    .ThenInclude(ct => ct.MaMhNavigation)
                .Where(h => h.TrangThai == Status.HoanThanh
                    && h.Ngay >= startDate
                    && h.Ngay <= endDate)
                .OrderBy(h => h.Ngay)
                .ToListAsync();

            // Tính tổng doanh thu
            var totalRevenue = orders.Sum(h => h.TongTien ?? 0);
            var totalOrders = orders.Count;
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Doanh thu theo sản phẩm (Top 10)
            var revenueByProduct = orders
                .SelectMany(h => h.Cthoadons)
                .GroupBy(ct => new
                {
                    ct.MaMhNavigation.MaMh,
                    ct.MaMhNavigation.Ten
                })
                .Select(g => new
                {
                    ProductName = g.Key.Ten,
                    Revenue = g.Sum(ct => (ct.DonGia ?? 0) * (ct.SoLuong ?? 0)),
                    Quantity = g.Sum(ct => ct.SoLuong ?? 0)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToList();

            // Doanh thu theo thời gian
            List<object> revenueByTime;

            switch (period)
            {
                case "month":
                    revenueByTime = orders
                        .GroupBy(h => new { h.Ngay.Value.Year, h.Ngay.Value.Month })
                        .Select(g => new
                        {
                            Period = $"{g.Key.Month:00}/{g.Key.Year}",
                            Revenue = g.Sum(h => h.TongTien ?? 0),
                            Orders = g.Count()
                        })
                        .OrderBy(x => x.Period)
                        .Cast<object>()
                        .ToList();
                    break;

                case "year":
                    revenueByTime = orders
                        .GroupBy(h => h.Ngay.Value.Year)
                        .Select(g => new
                        {
                            Period = g.Key.ToString(),
                            Revenue = g.Sum(h => h.TongTien ?? 0),
                            Orders = g.Count()
                        })
                        .OrderBy(x => x.Period)
                        .Cast<object>()
                        .ToList();
                    break;

                default: // day
                    revenueByTime = orders
                        .GroupBy(h => h.Ngay.Value.Date)
                        .Select(g => new
                        {
                            Period = g.Key.ToString("dd/MM/yyyy"),
                            Revenue = g.Sum(h => h.TongTien ?? 0),
                            Orders = g.Count()
                        })
                        .OrderBy(x => x.Period)
                        .Cast<object>()
                        .ToList();
                    break;
            }

            // Truyền dữ liệu sang view
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.Period = period;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.AvgOrderValue = avgOrderValue;
            ViewBag.RevenueByProduct = revenueByProduct;
            ViewBag.RevenueByTime = revenueByTime;

            return View();
        }

        // API: Lấy dữ liệu biểu đồ theo thời gian
        [HttpGet]
        public async Task<IActionResult> GetRevenueChartData(DateTime? startDate, DateTime? endDate, string period = "day")
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddDays(-7);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var orders = await _context.Hoadons
                .Where(h => h.TrangThai == Status.HoanThanh
                    && h.Ngay >= startDate
                    && h.Ngay <= endDate)
                .ToListAsync();

            object chartData;

            switch (period)
            {
                case "month":
                    chartData = orders
                        .GroupBy(h => new { h.Ngay.Value.Year, h.Ngay.Value.Month })
                        .Select(g => new
                        {
                            label = $"{g.Key.Month:00}/{g.Key.Year}",
                            revenue = g.Sum(h => h.TongTien ?? 0),
                            orders = g.Count()
                        })
                        .OrderBy(x => x.label)
                        .ToList();
                    break;

                case "year":
                    chartData = orders
                        .GroupBy(h => h.Ngay.Value.Year)
                        .Select(g => new
                        {
                            label = g.Key.ToString(),
                            revenue = g.Sum(h => h.TongTien ?? 0),
                            orders = g.Count()
                        })
                        .OrderBy(x => x.label)
                        .ToList();
                    break;

                default: // day
                    chartData = orders
                        .GroupBy(h => h.Ngay.Value.Date)
                        .Select(g => new
                        {
                            label = g.Key.ToString("dd/MM/yyyy"),
                            revenue = g.Sum(h => h.TongTien ?? 0),
                            orders = g.Count()
                        })
                        .OrderBy(x => x.label)
                        .ToList();
                    break;
            }

            return Json(chartData);
        }
    }
}

