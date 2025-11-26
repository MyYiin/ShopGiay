using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using ShopGiay.Data;
using ShopGiay.Models;
using System.IO;

namespace ShopGiay.Controllers
{
    public class MathangsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MathangsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: Mathangs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mathangs.Include(m => m.MaLgNavigation).Include(m => m.MaThNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Mathangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaLgNavigation)
                .Include(m => m.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }

            return View(mathang);
        }

        // GET: Mathangs/Create
        public IActionResult Create()
        {
            ViewData["MaLg"] = new SelectList(_context.Loaigiays, "MaLg", "Ten");
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "Ten");
            return View();
        }
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\images\\products\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }
        // POST: Mathangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile HinhAnh, [Bind("Ten,MoTa,GiaGoc,GiaBan,MaLg,MaTh")] Mathang mathang)
        {
            // Remove validation errors for properties we don't need from form
            ModelState.Remove("HinhAnh");
            ModelState.Remove("LuotXem");
            ModelState.Remove("LuotMua");
            ModelState.Remove("MaLgNavigation");
            ModelState.Remove("MaThNavigation");
            ModelState.Remove("Cthoadons");
            ModelState.Remove("Danhgia");
            ModelState.Remove("Tonkhos");

            if (ModelState.IsValid)
            {
                mathang.HinhAnh = Upload(HinhAnh);
                mathang.LuotXem = 0;
                mathang.LuotMua = 0;
                _context.Add(mathang);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã thêm mới sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Debug: Log validation errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            ViewData["MaLg"] = new SelectList(_context.Loaigiays, "MaLg", "Ten", mathang.MaLg);
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "Ten", mathang.MaTh);
            return View(mathang);
        }

        // GET: Mathangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs.FindAsync(id);
            if (mathang == null)
            {
                return NotFound();
            }
            ViewData["MaLg"] = new SelectList(_context.Loaigiays, "MaLg", "Ten", mathang.MaLg);
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "Ten", mathang.MaTh);
            return View(mathang);
        }

        // POST: Mathangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile HinhAnh, [Bind("MaMh,Ten,MoTa,GiaGoc,GiaBan,MaLg,MaTh,LuotXem,LuotMua")] Mathang mathang)
        {
            if (id != mathang.MaMh)
            {
                return NotFound();
            }

            // Remove validation cho navigation properties
            ModelState.Remove("MaLgNavigation");
            ModelState.Remove("MaThNavigation");
            ModelState.Remove("Cthoadons");
            ModelState.Remove("Danhgia");
            ModelState.Remove("Tonkhos");
            ModelState.Remove("HinhAnh");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy hình ảnh cũ từ database
                    var oldMathang = await _context.Mathangs.AsNoTracking().FirstOrDefaultAsync(m => m.MaMh == id);

                    // Nếu có upload hình mới thì dùng hình mới, không thì giữ hình cũ
                    if (HinhAnh != null)
                    {
                        mathang.HinhAnh = Upload(HinhAnh);
                    }
                    else
                    {
                        mathang.HinhAnh = oldMathang?.HinhAnh;
                    }

                    _context.Update(mathang);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã cập nhật sản phẩm thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MathangExists(mathang.MaMh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLg"] = new SelectList(_context.Loaigiays, "MaLg", "Ten", mathang.MaLg);
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "Ten", mathang.MaTh);
            return View(mathang);
        }

        // GET: Mathangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaLgNavigation)
                .Include(m => m.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }
            return View(mathang);
        }

        // POST: Mathangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mathang = await _context.Mathangs.FindAsync(id);
            if (mathang != null)
            {
                _context.Mathangs.Remove(mathang);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa sản phẩm thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MathangExists(int id)
        {
            return _context.Mathangs.Any(e => e.MaMh == id);
        }

        public async Task<IActionResult> TheoLoai(int id)
        {
            
            var ds = await _context.Mathangs
                .Where(m => m.MaLg == id)
                .ToListAsync();
           
            return View(ds);
        }
        public async Task<IActionResult> TheoThuongHieu(int id)
        {
           
            var ds = await _context.Mathangs
                .Where(m => m.MaTh == id)
                .ToListAsync();
           
            return View(ds);
        }


    }
}
