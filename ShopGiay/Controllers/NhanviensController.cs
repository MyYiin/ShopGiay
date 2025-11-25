using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;

namespace ShopGiay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NhanviensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Nhanvien> _passwordHasher;

        public NhanviensController(ApplicationDbContext context, IPasswordHasher<Nhanvien> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Nhanviens
        public async Task<IActionResult> Index()
        {
            return View(await _context.Nhanviens
                .Include(n => n.MaCvNavigation)
                .ToListAsync());
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,Ten,MaCv,DienThoai,Email,MatKhau")] Nhanvien nhanvien)
        {
            if (ModelState.IsValid)
            {
                // HASH mật khẩu
                nhanvien.MatKhau = _passwordHasher.HashPassword(nhanvien, nhanvien.MatKhau);

                _context.Add(nhanvien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv", nhanvien.MaCv);
            return View(nhanvien);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var nv = await _context.Nhanviens.FindAsync(id);
            if (nv == null)
                return NotFound();

            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv", nv.MaCv);
            return View(nv);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Nhanvien nhanvien, string newPassword)
        {
            if (id != nhanvien.MaNv)
                return NotFound();

            if (ModelState.IsValid)
            {
                var nvInDb = await _context.Nhanviens.FindAsync(id);

                nvInDb.Ten = nhanvien.Ten;
                nvInDb.MaCv = nhanvien.MaCv;
                nvInDb.Email = nhanvien.Email;
                nvInDb.DienThoai = nhanvien.DienThoai;

                // Hash mật khẩu nếu có nhập mật khẩu mới
                if (!string.IsNullOrEmpty(newPassword))
                {
                    nvInDb.MatKhau = _passwordHasher.HashPassword(nvInDb, newPassword);
                }

                _context.Update(nvInDb);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(nhanvien);
        }
    }
}
